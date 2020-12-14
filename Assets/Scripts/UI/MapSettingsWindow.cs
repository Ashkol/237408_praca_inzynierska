namespace MiniAstro.UI
{
    using UnityEngine;
    using MiniAstro.Management;
    using TMPro;
    using UnityEngine.UI;

    public class MapSettingsWindow : MonoBehaviour
    {
        [SerializeField] private TMP_InputField mapSeed;
        [SerializeField] private Button playButton;

        void Awake()
        {
            mapSeed.onEndEdit.AddListener((_) => PassSeedValue());
            SetLevel(2);
        }

        private void PassSeedValue()
        {
            int seed = SettingsManager.Instance.MapSettings.GetSeed();
            if (int.TryParse(mapSeed.text, out seed))
                SettingsManager.Instance.MapSettings.SetSeed(seed);
            else
                Debug.LogError($"Seed value not parsed - {seed}");
        }

        public void SetLevel(int i)
        {
            playButton.onClick.RemoveAllListeners();
            playButton.onClick.AddListener(() => GameManager.instance.LoadScene(i));
        }
    }

}
