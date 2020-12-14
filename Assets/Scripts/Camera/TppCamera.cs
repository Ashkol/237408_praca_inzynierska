namespace MiniAstro.Cameras
{
    using UnityEngine;

    public class TppCamera : MonoBehaviour
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 3 gameobjects, each parented to the next:

        // 	Camera Rig
        //      Pivot
        // 		    Camera

        [Range(0f, 10f)] [SerializeField] private float turnSpeed = 1.5f;   // How fast the rig will rotate from user input.

        float lookAngle;                    // The rig's y axis rotation.
        float tiltAngle;                    // The pivot's x axis rotation.
        Vector3 pivotEulers;
        Quaternion pivotTargetRot;
        private Quaternion transformTargetRot;

        protected Transform cam; // the transform of the camera
        protected Transform pivot; // the point at which the camera pivots around
        //private Transform swivel;
        protected Vector3 lastTargetPosition;
        public Transform target;
        public float turnSmoothing = 10;

        void Awake()
        {
            //base.Awake();
            cam = GetComponentInChildren<Camera>().transform;
            pivot = cam.parent;
            pivotEulers = pivot.rotation.eulerAngles;
            pivotTargetRot = pivot.transform.localRotation;
        }


        protected void FixedUpdate()
        {
            FollowTarget();
            HandleRotationMovement();
        }

        void FollowTarget()
        {
            if (target == null) return;
            transform.position = target.position;
        }

        private void HandleRotationMovement()
        {
            if (Time.timeScale < float.Epsilon)
                return;

            float x = Input.GetAxis("Mouse X");
            float y = Input.GetAxis("Mouse Y");

            if (Input.GetMouseButton(1))
            {
                // Adjust the look angle by an amount proportional to the turn speed and horizontal input.
                lookAngle += x * turnSpeed;

                // Rotate the rig (the root object) around Y axis only:
                transformTargetRot = Quaternion.Euler(0f, lookAngle , 0f);

                // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
                tiltAngle -= y * turnSpeed;
            }

            tiltAngle = Mathf.Clamp(tiltAngle, 0, 75);

            // Tilt input around X is applied to the pivot (the child of this object)
            pivotTargetRot = Quaternion.Euler(tiltAngle, pivotEulers.y, pivotEulers.z);
            if (turnSmoothing > 0 && Input.GetMouseButton(1))
            {
                pivot.localRotation = Quaternion.Slerp(pivot.localRotation, pivotTargetRot, turnSmoothing * Time.deltaTime);
                transform.localRotation = Quaternion.Slerp(transform.localRotation, transformTargetRot, turnSmoothing * Time.deltaTime);
            }
            else
            {
                pivot.localRotation = pivotTargetRot;
                transform.localRotation = transformTargetRot;
            }

        }
    }
}