namespace MiniAstro.Management
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    using AshkolTools.UI;

    public class GameManager : MonoBehaviour
    {
        public static GameManager instance;
        public GameSaveManager SaveManager { get; private set; }
        public GameObject loadingScreen;
        public ProgressBar progressBar;
        public GameObject postProcessing;

        SceneIndexes currentScene = SceneIndexes.MANAGER;

        void Awake()
        {
            instance = this;
            SaveManager = new GameSaveManager();
        }

        void Start()
        {
            SceneManager.sceneLoaded += SettingsManager.Instance.Video.OnSceneLoaded;
            SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);
            currentScene = SceneIndexes.TITLE_SCREEN;
        }

        List<AsyncOperation> scenesLoading = new List<AsyncOperation>();

        public void LoadGame()
        {
            loadingScreen.gameObject.SetActive(true);
            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
            //scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.GAME, LoadSceneMode.Additive));

            StartCoroutine(GetSceneLoadProgress());
        }

        public void LoadScene(int sceneIndex)
        {
            loadingScreen.gameObject.SetActive(true);
            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
            scenesLoading.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive));
            currentScene = (SceneIndexes)sceneIndex;
            //SceneManager.sceneLoaded += VideoSettingsManager.instance.OnSceneLoaded;

            StartCoroutine(GetSceneLoadProgress());
        }

        public void LoadMap()
        {
            loadingScreen.gameObject.SetActive(true);
            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)currentScene));
            int sceneIndex = SaveManager.MapData.typeOfMap == MapSettings.MapType.Plane ? (int)SceneIndexes.MARS : (int)SceneIndexes.MOON;
            scenesLoading.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive));
            currentScene = (SceneIndexes)sceneIndex;
            //SceneManager.sceneLoaded += VideoSettingsManager.instance.OnSceneLoaded;
            SceneManager.sceneLoaded += OnLoadedSaveMap;
            StartCoroutine(GetSceneLoadProgress());
        }

        private void OnLoadedSaveMap(Scene scene, LoadSceneMode mode)
        {
            var map = FindObjectOfType<TerrainGeneration.MapGenerator>();
            if (map != null)
                map.loadMap = true;
            SceneManager.sceneLoaded -= OnLoadedSaveMap;
        }


        public void ShowLoadingScreen(bool isShowing = true)
        {
            loadingScreen.gameObject.SetActive(isShowing);
        }

        public void LoadGame(int sceneIndex)
        {
            ShowLoadingScreen(true);
            scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
            scenesLoading.Add(SceneManager.LoadSceneAsync(sceneIndex, LoadSceneMode.Additive));
            currentScene = (SceneIndexes)sceneIndex;
            //SceneManager.sceneLoaded += VideoSettingsManager.instance.OnSceneLoaded;

            StartCoroutine(GetSceneLoadProgress());
        }


        float totalSceneProgress;
        public IEnumerator GetSceneLoadProgress()
        {
            for (int i = 0; i < scenesLoading.Count; i++)
            {
                while (!scenesLoading[i].isDone)
                {
                    totalSceneProgress = 0;

                    foreach (AsyncOperation operation in scenesLoading)
                    {
                        totalSceneProgress += operation.progress;
                    }
                    totalSceneProgress = (totalSceneProgress / scenesLoading.Count) * 100f;
                    progressBar.current = Mathf.RoundToInt(totalSceneProgress);
                    yield return null;
                }
            }

            loadingScreen.gameObject.SetActive(false);
            scenesLoading.Clear();
        }
        public void Quit()
        {
            SettingsManager.Instance.Save();
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
		    Application.Quit();
#endif
        }

        public void SetPostProcessing(bool postProcessingOn)
        {
            postProcessing.SetActive(postProcessingOn);
        }

    }

}
