namespace MiniAstro.Management
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class SettingsManager : MonoBehaviour
    {
        public static SettingsManager Instance { get; set; }
        private Settings settings;

        public VideoSettingsManager Video { get { return _video; } private set { _video = value; } }
        [SerializeField] private VideoSettingsManager _video;
        public AudioSettingsManager Audio { get { return _audio; } private set { _audio = value; } }
        [SerializeField] private AudioSettingsManager _audio;

        public MapSettingsManager MapSettings { get { return _mapSettings; } private set { _mapSettings = value; } }
        [SerializeField] private MapSettingsManager _mapSettings;

        void Awake()
        {
            Instance = this;
            settings = new Settings();
            settings.Load();
            Video.settings = this.settings;
            Audio.settings = this.settings;
            Debug.Log(settings);
        }

        public void Save()
        {
            settings.Save();
            Debug.Log(settings);
        }

    }
    
}