namespace MiniAstro.Player
{
    using UnityEngine;
    public class PlayerMovement : MonoBehaviour
    {
        private Animator anim;
        private CharacterController controller;
        private Rigidbody rgbody;


        public float speed = 5f;
        public float gravity = 9.81f;
        public float jumpHeight = 5f;
        public Transform groundCheck;
        public Camera tppCamera;
        private bool isGrounded;
        private float animSpeed;


        void Start()
        {
            controller = GetComponent<CharacterController>();
            anim = gameObject.GetComponentInChildren<Animator>();
            animSpeed = anim.speed;
            rgbody = GetComponent<Rigidbody>();
        }

        void FixedUpdate()
        {
            //isGrounded = Physics.CheckSphere(transform.position, 0.2f, Ground, QueryTriggerInteraction.Ignore);
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, 262144, QueryTriggerInteraction.Ignore);
            if (isGrounded)
            {
                if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)
                {
                    anim.SetInteger("AnimationPar", 1);
                }
                else
                {
                    anim.SetInteger("AnimationPar", 0);
                }
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
                
            rgbody.MovePosition(rgbody.position + tppCamera.transform.TransformDirection(moveDir) * moveSpeed * Time.fixedDeltaTime);
        }

        void Jump()
        {
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                rgbody.AddForce(Vector3.up * Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y), ForceMode.VelocityChange);
            }
        }

        void Rotate()
        {
            Vector3 rotDir = (new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))).normalized;
            rotDir = tppCamera.transform.TransformDirection(rotDir);
            rotDir.y = 0;
            if (rotDir != Vector3.zero)
                rgbody.MoveRotation(Quaternion.LookRotation(rotDir, Vector3.up));

        }

        //void Update()
        //{
        //    //if (Input.GetKey("w"))
        //    if (Input.GetAxis("Vertical") != 0f || Input.GetAxis("Horizontal") != 0f)
        //    {
        //        anim.SetInteger("AnimationPar", 1);
        //    }
        //    else
        //    {
        //        anim.SetInteger("AnimationPar", 0);
        //    }

        //    if (controller.isGrounded)
        //    {
        //        //moveDirection = transform.forward * Input.GetAxis("Vertical") * speed;
        //        moveDirection = new Vector3(Input.GetAxis("Vertical"), 0f, Input.GetAxis("Horizontal")) * speed;
        //    }

        //    //float turn = Input.GetAxis("Horizontal");
        //    //transform.Rotate(0, turn * turnSpeed * Time.deltaTime, 0);
        //    Vector3 dirVector = (new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"))).normalized;
        //    if (dirVector != Vector3.zero)
        //        rgbody.MoveRotation(Quaternion.LookRotation(dirVector, Vector3.up));
        //    controller.Move(moveDirection * Time.deltaTime);
        //    moveDirection.y -= gravity * Time.deltaTime;
        //}
    }
}
