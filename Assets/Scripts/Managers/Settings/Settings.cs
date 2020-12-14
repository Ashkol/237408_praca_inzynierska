namespace MiniAstro.Management
{
    using UnityEngine;

    public class Settings
    {
        private const string MUSIC_VOLUME_KEY = "music_volume";
        private const string POST_PROCESSING_ON_KEY = "post_processing_on";
        private const string FPS_DISPLAY_ON_KEY = "fps_display_on";

        public float MusicVolume
        {
            get { return _musicVolume; }
            set { _musicVolume = Mathf.Clamp01(value); }
        }
        private float _musicVolume;
        public bool PostProcessingOn { get; set; }
        public bool FPSDisplayOn { get; set; }

        public void ResetToDefault()
        {
            MusicVolume = 1f;
            PostProcessingOn = true;
            FPSDisplayOn = true;
        }

        public void Save()
        {
            CreateMissingKeys();

            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, MusicVolume);
            PlayerPrefs.SetInt(POST_PROCESSING_ON_KEY, PostProcessingOn ? 1 : 0);
            PlayerPrefs.SetInt(FPS_DISPLAY_ON_KEY, FPSDisplayOn ? 1 : 0);

            PlayerPrefs.Save();
        }

        public void Load()
        {
            CreateMissingKeys();

            MusicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY);
            PostProcessingOn = PlayerPrefs.GetInt(POST_PROCESSING_ON_KEY) == 1;
            FPSDisplayOn = PlayerPrefs.GetInt(FPS_DISPLAY_ON_KEY) == 1;
        }

        private void CreateMissingKeys()
        {
            bool isKeyCreated = false;
            if (!PlayerPrefs.HasKey(MUSIC_VOLUME_KEY))
            {
                PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, 1f);
                isKeyCreated = true;
            }
            if (!PlayerPrefs.HasKey(POST_PROCESSING_ON_KEY))
            {
                PlayerPrefs.SetInt(POST_PROCESSING_ON_KEY, 1);
                isKeyCreated = true;
            }
            if (!PlayerPrefs.HasKey(FPS_DISPLAY_ON_KEY))
            {
                PlayerPrefs.SetInt(FPS_DISPLAY_ON_KEY, 1);
                isKeyCreated = true;
            }

            if (isKeyCreated)
            {
                PlayerPrefs.Save();
            }
        }

        public override string ToString()
        {
            string log = $"Settings:\nMusic volume: {MusicVolume * 100}%\nPost processing: {(PostProcessingOn ? "On" : "Off")}\nFPS display: {(FPSDisplayOn ? "On" : "Off")}";
            return log;
        }

    }


}