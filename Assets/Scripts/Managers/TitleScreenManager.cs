namespace MiniAstro.Management
{
    using UnityEngine;
    using UnityEngine.UI;
    public class TitleScreenManager : MonoBehaviour
    {
        [Header("Menu")]
        public Button newGameBtn;
        public Button continueBtn;
        public Button marsGameBtn;
        public Button moonGameBtn;
        public Button settingsBtn;
        public Button quitBtn;

        [Header("Video")]
        public Toggle postProcessingToggle;
        public Toggle fpsDisplayToggle;

        [Header("Audio")]
        public Slider musicVolumeSlider;


        void Awake()
        {
            //newGameBtn.onClick.AddListener(() => )

            if (moonGameBtn != null)
                moonGameBtn.onClick.AddListener(() => { GameManager.instance.LoadGame((int)SceneIndexes.MOON); });
            if (marsGameBtn != null)
                marsGameBtn.onClick.AddListener(() => { GameManager.instance.LoadGame((int)SceneIndexes.MARS); });
            quitBtn.onClick.AddListener(GameManager.instance.Quit);

            SetVideoControls();
            SetAudioControls();
        }

        void SetVideoControls()
        {
            postProcessingToggle.onValueChanged.AddListener(SettingsManager.Instance.Video.PostProcessingActive);
            postProcessingToggle.isOn = SettingsManager.Instance.Video.settings.PostProcessingOn;
            fpsDisplayToggle.onValueChanged.AddListener(SettingsManager.Instance.Video.FPSDisplayActive);
            fpsDisplayToggle.isOn = SettingsManager.Instance.Video.settings.FPSDisplayOn;

            SettingsManager.Instance.Video.ApplySettings();
        }

        void SetAudioControls()
        {
            musicVolumeSlider.onValueChanged.AddListener(SettingsManager.Instance.Audio.SetMusicVolume);
            musicVolumeSlider.value = SettingsManager.Instance.Audio.MusicVolume;

            SettingsManager.Instance.Audio.ApplySettings();
        }
    }
}