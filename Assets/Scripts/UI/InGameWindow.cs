using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MiniAstro.UI
{
    public abstract class InGameWindow : MonoBehaviour
    {
        protected RectTransform[] children;
        protected bool windowOn = false;
        protected abstract void Initialize();
        protected abstract void Open();
        protected abstract void Close();
        protected abstract void PauseGame();
        protected abstract void ResumeGame();
    }
}


