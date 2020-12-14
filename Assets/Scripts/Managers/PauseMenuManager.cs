namespace MiniAstro.Management
{
    using UnityEngine;
    using UnityEngine.UI;
    public class PauseMenuManager : MonoBehaviour
    {
        public Button continueBtn;
        public Button newGameBtn;
        public Button settingsBtn;
        public Button quitBtn;


        void Awake()
        {
            newGameBtn.onClick.AddListener(() => { GameManager.instance.LoadGame((int)SceneIndexes.TITLE_SCREEN); });
            settingsBtn.onClick.AddListener(this.ShowSettings);
            quitBtn.onClick.AddListener(GameManager.instance.Quit);
        }

        void ShowSettings()
        {
            // TO DO
        }

        
    }

}
