namespace MiniAstro.UI
{
    using MiniAstro.Management;
    using MiniAstro.TerrainGeneration;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class LoadGameWindow : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Transform windowContent;
        [SerializeField] private Button contentButtonPrefab;
        private GameSaveManager loadManager;

        public void Awake()
        {
            loadManager = GameManager.instance.SaveManager;
            if (loadManager == null)
                loadManager = new GameSaveManager();

            SetFetchedSavesNames();
        }

        public void OnEnable()
        {
            //SetFetchedSavesNames();
        }

        public void LoadGame()
        {
            GameManager.instance.ShowLoadingScreen(true);
            GameManager.instance.SaveManager.Load(inputField.text);
            GameManager.instance.LoadMap();
            //GameManager.instance.LoadGame((int)loadManager.MapData.typeOfMap + 2);
        }

        private void SetFetchedSavesNames()
        {
            var saves = loadManager.FetchSaveFiles();
            foreach (string saveName in saves)
            {
                var saveButton = (Button)Instantiate(contentButtonPrefab, windowContent);
                if (saveButton.GetComponentInChildren<TextMeshProUGUI>())
                {
                    var buttonText = saveButton.GetComponentInChildren<TextMeshProUGUI>();
                    buttonText.text = saveName;
                }
                else
                {
                    var buttonText = saveButton.GetComponentInChildren<Text>();
                    buttonText.text = saveName;
                }
                saveButton.onClick.AddListener(() => inputField.text = saveName);
            }
        }
    }

}
