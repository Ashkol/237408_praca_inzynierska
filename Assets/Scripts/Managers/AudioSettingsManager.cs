namespace MiniAstro.Management
{
    using UnityEngine;

    public class AudioSettingsManager : MonoBehaviour
    {
        AudioSource audioSource;
        public Settings settings;

        public float MusicVolume
        {
            get { return settings.MusicVolume; }
            set
            {
                settings.MusicVolume = value;
                audioSource.volume = settings.MusicVolume;
            }
        }

        void Awake()
        {
            if (audioSource == null)
                audioSource = GetComponent<AudioSource>();
        }

        public void SetMusicVolume(float volume)
        {
            MusicVolume = volume;
        }

        public void ApplySettings()
        {
            SetMusicVolume(settings.MusicVolume);
        }
    }
}