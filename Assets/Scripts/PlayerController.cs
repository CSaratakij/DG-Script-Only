using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        bool isInverseMovement;

        [SerializeField]
        float moveForce;

        [SerializeField]
        float jumpForce;

        [SerializeField]
        float gravity;

        [SerializeField]
        LayerMask groundMask;


        bool isCanJump;
        bool isGrounded;
        bool isFlipX;

        Vector2 input;
        Vector2 velocity;

        Animator anim;
        Rigidbody2D rigid;
        Transform ground;


        void Awake()
        {
            _Initialize();
        }

        void Start()
        {
            if (!ground) {
                Debug.Log("Cannot find ground..");
            }
        }

        void Update()
        {
            _InputHandler();
            _AnimationHandler();
        }

        void FixedUpdate()
        {
            _MovementHandler();
        }


        void _Initialize()
        {
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            ground = transform.Find("ground");
        }

        void _InputHandler()
        {
            input.x = Input.GetAxisRaw("Horizontal");

            if (isInverseMovement) {
                input.x *= -1.0f;
            }

            if (Input.GetButtonDown("Jump")) {
                isCanJump = true;

                if (isGrounded && isCanJump) {
                    velocity.y = jumpForce;
                    rigid.velocity = velocity;
                }
            }
            else if (Input.GetButtonUp("Jump")) {
                isCanJump = false;
            }

            _FlipXHandler();

        }

        void _AnimationHandler()
        {
            if (isGrounded) {
                if (Input.GetAxisRaw("Horizontal") != 0.0f) {
                    anim.Play("Run");
                }
                else {
                    anim.Play("Falling Impact");
                }
            }
            else {
                anim.Play("Fall");
            }
        }

        void _MovementHandler()
        {
            isGrounded = Physics2D.OverlapCircle(ground.position, 0.02f, groundMask);
            velocity.x = input.x * moveForce; 
            velocity.y = rigid.velocity.y;
            velocity.y -= gravity * Time.deltaTime;
            rigid.velocity = velocity;
        }

        void _FlipXHandler()
        {
            var newScale = transform.localScale;

            if (input.x >= 1.0f) {
                newScale.x = 1.0f;

            } else if (input.x <= -1.0f) {
                newScale.x = -1.0f;
            }

            transform.localScale = newScale;
        }
    }
}
