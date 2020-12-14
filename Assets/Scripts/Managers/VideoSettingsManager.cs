namespace MiniAstro.Management
{
    using UnityEngine;
    using UnityEngine.Rendering.PostProcessing;
    using UnityEngine.SceneManagement;
    using AshkolTools.Utils;

    public class VideoSettingsManager : MonoBehaviour
    {

        [Header("Postprocessing")]
        public PostProcessVolume postProcessVolume;
        public bool postProcessingActive = true;

        [Header("FPS Display")]
        public FPSDisplay fpsDisplay;

        public Settings settings;

        public void PostProcessingActive(bool isActive)
        {
            settings.PostProcessingOn = isActive;
            if (postProcessVolume != null)
                postProcessVolume.enabled = settings.PostProcessingOn;
            postProcessingActive = settings.PostProcessingOn;
        }

        public void FPSDisplayActive(bool isActive)
        {
            settings.FPSDisplayOn = isActive;
            fpsDisplay.enabled = settings.FPSDisplayOn;
        }

        public void OnSceneLoaded(Scene scene, LoadSceneMode Mode)
        {
            // TODO move the following line to GameManager
            SceneManager.SetActiveScene(scene);

            postProcessVolume = FindObjectOfType<PostProcessVolume>();
            PostProcessingActive(settings.PostProcessingOn);
        }

        public void ApplySettings()
        {
            FPSDisplayActive(settings.FPSDisplayOn);
        }
    }
}