namespace MiniAstro.Environment
{
    using UnityEngine;
    using UnityEngine.UI;
    using MiniAstro.Management;

    public class ActionObject : MonoBehaviour
    {
        public Transform labelPrefab;
        public Camera cam;
        Transform label;
        public GameObject winMessage;
        public Button endLevelBtn;

        void Awake()
        {
            label = Instantiate(labelPrefab, transform);
            label.gameObject.SetActive(false);
        }

        public void OnMouseEnter()
        {
            label.gameObject.SetActive(true);
        }

        public void OnMouseExit()
        {
            label.gameObject.SetActive(false);
        }

        public void OnMouseOver()
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, Mathf.Infinity))
            {
                DisplayLabel(hit);
            }
            else
            {
                HideLabel();
            }
            if (Input.GetKeyDown(KeyCode.B))
            {
                if (ScoreManager.instance.LevelFinished)
                {
                    winMessage.SetActive(true);
                    Time.timeScale = 0f;
                    endLevelBtn.onClick.AddListener(() => { GameManager.instance.LoadScene((int)SceneIndexes.TITLE_SCREEN); });
                    endLevelBtn.onClick.AddListener(() => { Time.timeScale = 1f; });
                }
            }
        }

        void DisplayLabel(RaycastHit hit)
        {
            label.gameObject.SetActive(true);
            //Vector3 cursorHitScreenPos = cam.WorldToScreenPoint(hit.point);
            label.transform.position = Vector3.Lerp(hit.point, cam.transform.position, 0.1f);
            label.transform.LookAt(cam.transform);
        }

        void HideLabel()
        {
            label.gameObject.SetActive(false);
        }
    }
}