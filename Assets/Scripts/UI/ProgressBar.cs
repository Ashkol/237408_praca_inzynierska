namespace AshkolTools.UI
{
    using UnityEngine;
    using UnityEngine.UI;
#if UNITY_EDITOR
    using UnityEditor;
#endif

    [ExecuteInEditMode()]
    public class ProgressBar : MonoBehaviour
    {
        public int minimum;
        public int maximum;
        public int current;
        public Image mask;
        public Image fill;
        public Color fillColor;

        void Update()
        {
            UpdateFill();
        }

        void UpdateFill()
        {
            float currentOffset = current - minimum;
            float maximumOffset = maximum - minimum;
            float fillAmount = (float)currentOffset / (float)maximumOffset;
            mask.fillAmount = fillAmount;
            fill.color = fillColor;
        }
    }

}
