namespace MiniAstro.Cameras
{
    using UnityEngine;
    using MiniAstro.Physics.SphericalGravity;

    public class TppCameraSpherical : MonoBehaviour
    {
        // This script is designed to be placed on the root object of a camera rig,
        // comprising 4 gameobjects, each parented to the next:

        // 	Camera Rig
        //      Swivel
        // 	    	Pivot
        // 		    	Camera

        Transform planet;
        [Range(0f, 10f)] [SerializeField] private float TurnSpeed = 1.5f;   // How fast the rig will rotate from user input.

        float lookAngle;                    // The rig's y axis rotation.
        float tiltAngle;                    // The pivot's x axis rotation.
        Vector3 pivotEulers;
        Quaternion pivotTargetRot;
        Quaternion swivelTargetRot;

        protected Transform cam; // the transform of the camera
        protected Transform pivot; // the point at which the camera pivots around
        private Transform swivel;
        protected Vector3 lastTargetPosition;
        public Transform target;
        public float turnSmoothing = 10;

        void Awake()
        {
            //base.Awake();
            cam = GetComponentInChildren<Camera>().transform;
            pivot = cam.parent;
            swivel = pivot.parent;
            // Lock or unlock the cursor.
            pivotEulers = pivot.rotation.eulerAngles;

            pivotTargetRot = pivot.transform.localRotation;
            swivelTargetRot = transform.localRotation;
            planet = FindObjectOfType<GravityAttractor>().transform;
        }


        protected void FixedUpdate()
        {
            FollowTarget();
            HandleRotationMovement();
            //AttractSelf(planet);
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
                lookAngle = x * TurnSpeed;

                // Rotate the rig (the root object) around Y axis only:
                swivelTargetRot = Quaternion.Euler(0f, lookAngle, 0f);

                // on platforms with a mouse, we adjust the current angle based on Y mouse input and turn speed
                tiltAngle -= y * TurnSpeed;
            }

            if (Input.GetMouseButtonUp(1))
            {
                lookAngle = 0;
                swivelTargetRot = Quaternion.Euler(0f, lookAngle, 0f);
                //TransformTargetRot = Quaternion.Euler(0f, LookAngle, 0f);
            }

            // Tilt input around X is applied to the pivot (the child of this object)
            pivotTargetRot = Quaternion.Euler(tiltAngle, pivotEulers.y, pivotEulers.z);
            if (turnSmoothing > 0 && Input.GetMouseButton(1))
            {
                pivot.localRotation = Quaternion.Slerp(pivot.localRotation, pivotTargetRot, turnSmoothing * Time.deltaTime);
                swivel.localRotation = Quaternion.Slerp(swivel.localRotation, swivelTargetRot, turnSmoothing * Time.deltaTime);
            }
            else
            {
                pivot.localRotation = pivotTargetRot;
                swivel.localRotation = swivelTargetRot;
            }

            AttractSelf(planet);
        }

        void AttractSelf(Transform attractor)
        {
            Vector3 targetDir = (transform.position - attractor.position).normalized;
            transform.localRotation = Quaternion.LookRotation(Vector3.Cross(targetDir, -swivel.right), targetDir);
        }
    }
}