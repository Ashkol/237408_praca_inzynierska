namespace MiniAstro.Cameras
{
    using UnityEngine;

    public class RotatingSkyboxCamera : MonoBehaviour
    {
        [Tooltip("Day/night cycle time in minutes")]
        public float dayNightCycle = 2;
        // set the main camera in the inspector
        public Camera mainCamera;

        // set the sky box camera in the inspector
        public Camera skyCamera;

        // Use this for initialization
        void Start()
        {
            if (skyCamera.depth >= mainCamera.depth)
            {
                Debug.Log("Set skybox camera depth lower " +
                    " than main camera depth in inspector");
            }
            if (mainCamera.clearFlags != CameraClearFlags.Nothing)
            {
                Debug.Log("Main camera needs to be set to dont clear" +
                    "in the inspector");
            }
            // If skycamera was not set in the inspector look for it in the object itself
            if (skyCamera == null)
                skyCamera = GetComponent<Camera>();
        }

        void Rotate()
        {
            float angle = (360 / (dayNightCycle * 60)) * Time.deltaTime;
            skyCamera.transform.RotateAround(transform.position, -Vector3.up, angle);
            //SkyCamera.transform.RotateAround(Vector3.zero, -MainCamera.transform.up, angle);
        }

        // Update is called once per frame
        void Update()
        {
            Rotate();
        }
    }
}