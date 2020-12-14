namespace MiniAstro.Player
{
    using UnityEngine;
    using MiniAstro.Physics.SphericalGravity;

    public class PlayerMovementSpherical : MonoBehaviour
    {
        private Animator anim;
        private Rigidbody rgbody;
        GravityAttractor planet;

        public float speed = 2f;
        public float jumpHeight = 5f;
        public Transform groundCheck;
        public Transform tppCamera;
        public Transform yRotationRoot;
        private bool isGrounded;
        private float animSpeed;

        void Start()
        {
            anim = gameObject.GetComponentInChildren<Animator>();
            animSpeed = anim.speed;
            rgbody = GetComponent<Rigidbody>();

            planet = FindObjectOfType<GravityAttractor>();
            rgbody.useGravity = false;
            rgbody.constraints = RigidbodyConstraints.FreezeRotation;
        }

        void FixedUpdate()
        {
            // terrain mesh layer - 18th
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, 262144, QueryTriggerInteraction.Ignore);
            
            if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)
            {
                anim.SetInteger("AnimationPar", 1);
            }
            else
            {
                anim.SetInteger("AnimationPar", 0);
            }

            Move();
            Jump();
            Rotate();
        }

        void Move()
        {
            float moveSpeed = speed;
            if (Input.GetKey(KeyCode.LeftShift))
            {
                moveSpeed = speed * 2;
                anim.speed = animSpeed * 2;
            }
            else
            {
                anim.speed = animSpeed;
            }

            Vector3 moveDir = (new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))).normalized;

            if (transform.position.y > 0)
                rgbody.MovePosition(rgbody.position + /*tppCamera.*/transform.TransformDirection(moveDir) * moveSpeed * Time.fixedDeltaTime);
            else
                rgbody.MovePosition(rgbody.position + /*tppCamera.*/transform.TransformDirection(moveDir) * moveSpeed * Time.fixedDeltaTime);
        }

        void Jump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rgbody.AddForce(transform.up * Mathf.Sqrt(jumpHeight * -2f * planet.gravity), ForceMode.VelocityChange);
            }
        }

        void Rotate()
        {
            Vector3 targetDir = (transform.position - planet.transform.position).normalized;
            Vector3 moveDir = (new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"))).normalized;

            if (Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0)
            {
                //rgbody.MoveRotation(Quaternion.FromToRotation(transform.up, targetDir) * transform.rotation);
                //rgbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(targetDir, -transform.right), targetDir));

                rgbody.MoveRotation(Quaternion.LookRotation(Vector3.Cross(targetDir, -tppCamera.right), targetDir));

                float angle = Vector3.Angle(Vector3.forward, moveDir);
                if (Input.GetAxis("Horizontal") < 0)
                    angle *= -1;
                yRotationRoot.localRotation = Quaternion.Euler(0, angle, 0);
            }
            planet.Attract(rgbody, false);
        }

    }
}
