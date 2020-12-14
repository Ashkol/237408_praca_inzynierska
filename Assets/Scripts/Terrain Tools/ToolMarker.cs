namespace MiniAstro.TerrainGeneration
{
    using UnityEngine;

    public class ToolMarker : MonoBehaviour
    {
        public Transform arrowDown;
        public Transform arrowUp;
        public Transform flatBlock;
        [SerializeField] Renderer circleRenderer = default;

        public enum MarkerType
        {
            up,
            down,
            flat
        }

        MarkerType _marker = default;
        public MarkerType Marker
        {
            get { return _marker; }
            set
            {
                switch (value)
                {
                    case MarkerType.up:
                        arrowUp.gameObject.SetActive(true);
                        arrowDown.gameObject.SetActive(false);
                        flatBlock.gameObject.SetActive(false);
                        break;
                    case MarkerType.down:
                        arrowUp.gameObject.SetActive(false);
                        arrowDown.gameObject.SetActive(true);
                        flatBlock.gameObject.SetActive(false);
                        break;
                    case MarkerType.flat:
                        arrowUp.gameObject.SetActive(false);
                        arrowDown.gameObject.SetActive(false);
                        flatBlock.gameObject.SetActive(true);
                        break;
                    default:
                        break;
                }
            }
        }

        public void SetColor(Color color)
        {
            if (circleRenderer.material.color != color)
                circleRenderer.material.color = color;
        }

        void OnDrawGizmos()
        {
            Gizmos.DrawWireSphere(transform.position, FindObjectOfType<TerrainTool>().radius);
        }
    }
}
