namespace MiniAstro.UI
{
    using UnityEngine;
    public class InventoryWindow : InGameWindow
    {
        void Awake()
        {
            Initialize();
        }

        protected override void Initialize()
        {
            RectTransform[] temp = GetComponentsInChildren<RectTransform>();
            children = new RectTransform[temp.Length - 1];
            for (int i = 1; i < temp.Length; i++)
            {
                children[i - 1] = temp[i];
            }

            //foreach (RectTransform child in children)
            //{
            //    child.gameObject.SetActive(false);
            //}
            children[0].gameObject.SetActive(false);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (!windowOn)
                    Open();
                else
                    Close();
            }
        }

        protected override void Open()
        {
            foreach (RectTransform child in children)
            {
                child.gameObject.SetActive(true);
            }

            windowOn = true;
            PauseGame();
        }

        protected override void Close()
        {
            foreach (RectTransform child in children)
            {
                child.gameObject.SetActive(false);
            }

            windowOn = false;
            ResumeGame();
        }

        protected override void ResumeGame()
        {
        }

        protected override void PauseGame()
        {
        }

    }

}
