namespace MiniAstro.UI
{
    using UnityEngine;
    using UnityEngine.UI;
    using MiniAstro.Management;
    using MiniAstro.TerrainGeneration;

    public class PauseMenu : InGameWindow
    {
        public Button continueBtn;
        public Button mainMenuBtn;
        public Button settingsBtn;
        public Button quitBtn;

        //bool pauseMenuOn = false;


        void Start()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            continueBtn.onClick.AddListener(ResumeGame);
            continueBtn.onClick.AddListener(Close);
            mainMenuBtn.onClick.AddListener(() => { GameManager.instance.LoadScene((int)SceneIndexes.TITLE_SCREEN); });
            mainMenuBtn.onClick.AddListener(ResumeGame);
            if (GameManager.instance != null)
                quitBtn.onClick.AddListener(GameManager.instance.Quit);

            RectTransform[] temp = GetComponentsInChildren<RectTransform>();
            children = new RectTransform[temp.Length - 1];
            for (int i = 1; i < temp.Length; i++)
            {
                children[i - 1] = temp[i];
            }

            foreach (RectTransform child in children)
            {
                //Debug.LogWarning
                child.gameObject.SetActive(false);
            }
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!windowOn)
                    Open();
                else
                    Close();
            }
        }

        protected override void Open()
        {
            foreach (RectTransform child in children)
            {
                child.gameObject.SetActive(true);
            }
            FindObjectOfType<MouseTerrainTool>().enabled = false;
            Cursor.visible = true;

            windowOn = true;
            PauseGame();
        }

        protected override void Close()
        {
            foreach (RectTransform child in children)
            {
                child.gameObject.SetActive(false);
            }
            FindObjectOfType<MouseTerrainTool>().enabled = true;
            Cursor.visible = false;

            windowOn = false;
            ResumeGame();
        }

        protected override void ResumeGame()
        {
            Time.timeScale = 1f;
        }

        protected override void PauseGame()
        {
            //Time.timeScale = 0f;
        }
    }
}