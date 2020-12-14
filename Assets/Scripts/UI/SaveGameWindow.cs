namespace MiniAstro.UI
{
    using MiniAstro.Management;
    using MiniAstro.TerrainGeneration;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;

    public class SaveGameWindow : MonoBehaviour
    {
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private MapGenerator map;
        [SerializeField] private Transform windowContent;
        [SerializeField] private Button contentButtonPrefab;
        private GameSaveManager saveManager;

        public void Awake()
        {
            saveManager = GameManager.instance.SaveManager;
            if (saveManager == null)
                saveManager = new GameSaveManager();

            if (map == null)
            {
                map = FindObjectOfType<MapGenerator>();
            }

            SetFetchedSavesNames();
        }

        public void SaveGame()
        {
            saveManager.Save(map.MapSettings, map.Chunks.ToArray(), inputField.text);
        }

        private void SetFetchedSavesNames()
        {
            var saves = saveManager.FetchSaveFiles();
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
