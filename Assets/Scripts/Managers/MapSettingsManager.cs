namespace MiniAstro.Management
{
    using UnityEngine;

    public class MapSettingsManager : MonoBehaviour
    {
        public MapSettings Settings { get; private set; }
        public bool UseRandomSeed { get; set; }

        private void Awake()
        {
            Settings = new MapSettings();
        }

        #region Seed
        public void SetSeed(int seed)
        {
            Settings.Seed = seed;
        }

        public int GetSeed()
        {
            if (UseRandomSeed)
            {
                var rng = new System.Random();
                return rng.Next();
            }
            else
            {
                return Settings.Seed;
            }
        }
        #endregion
    }
}



