namespace MiniAstro.Environment
{
    using UnityEngine;
    using MiniAstro.Physics.SphericalGravity;

    public class SurfaceMarker : MonoBehaviour
    {
        LineRenderer lineRenderer;
        public Transform icon;
        Transform player;
        GravityAttractor planet;

        void Awake()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            player = GameObject.FindGameObjectsWithTag("Player")[0].transform;
            planet = FindObjectOfType<GravityAttractor>();
        }

        void Update()
        {
            RotateIcon();
        }

        void RotateIcon()
        {
            float distance = Mathf.Clamp(Vector3.Distance(player.position, transform.position) * 0.7f, 1f, 100f);
            icon.localPosition = Vector3.up * distance;
            //icon.LookAt(player.position);
            Vector3[] points = { Vector3.zero, Vector3.up * distance };
            lineRenderer.SetPositions(points);
        }
    }
}