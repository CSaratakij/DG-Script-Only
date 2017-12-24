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
        float maxVelocityY;

        [SerializeField]
        float fallMultiplier;

        [SerializeField]
        float lowJumpMultiplier;

        [SerializeField]
        LayerMask groundMask;

        [SerializeField]
        LayerMask footstepMask;

        [SerializeField]
        AudioClip jumpSound;


        bool isPressedJump;
        bool isGrounded;
        bool isFalling;

        Vector2 input;
        Vector2 velocity;

        Animator anim;
        Rigidbody2D rigid;

        AudioSource audioSource;
        FootStepAudioPlayer footStepAudioPlayer;

        Transform ground;
        Transform feet;

        RaycastHit2D materialRay;
        CameraFolllow cameraFollow;

        Vector3 newScale;


        void Awake()
        {
            _Initialize();
        }

        void Start()
        {
            if (!ground) {
                Debug.Log("Cannot find ground..");
            }

            if (audioSource && jumpSound) {
                audioSource.clip = jumpSound;
            }
            else {
                Debug.Log("Cannot find audioSource for jump or cannot find jump sound clip..");
            }

            if (isInverseMovement) {
                var newScale = transform.localScale;
                newScale.x = -1.0f;
                transform.localScale = newScale;
            }

            anim.Play("Idle");
        }

        void Update()
        {
            if (rigid.velocity.y < 0.0f) {
                isFalling = true;
            }

            _InputHandler();
            _FlipXHandler();
            _AnimationHandler();
            _FootStepHandler();

            //Temp
            _ResetPosition();
        }

        void FixedUpdate()
        {
            _JumpHandler();
            _MovementHandler();
        }

        void LateUpdate()
        {
            if (isGrounded && !cameraFollow.IsOutBoundY) {
                cameraFollow.UnStickYAxis();
            }
        }

        void _Initialize()
        {
            anim = GetComponent<Animator>();
            rigid = GetComponent<Rigidbody2D>();
            audioSource = GetComponent<AudioSource>();
            ground = transform.Find("ground");
            feet = transform.Find("footstep");
            footStepAudioPlayer = transform.Find("footstep").gameObject.GetComponent<FootStepAudioPlayer>();
            cameraFollow = Camera.main.GetComponent<CameraFolllow>();
            newScale = transform.localScale;
        }

        void _InputHandler()
        {
            input.x = Input.GetAxisRaw("Horizontal");

            if (isInverseMovement) {
                input.x *= -1.0f;
            }

            if (Input.GetButtonDown("Jump")) {
                isPressedJump = true;
            }

            //test
            if (Input.GetKeyDown(KeyCode.L)) {
                cameraFollow.LockCamera();
            }
            else if (Input.GetKeyDown(KeyCode.U)) {
                cameraFollow.UnlockCamera();
            }
        }

        void _AnimationHandler()
        {
            if (isGrounded) {
                if (Input.GetAxisRaw("Horizontal") != 0.0f) {
                    anim.Play("Run");
                }
                else {
                    if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                        anim.Play("Falling Impact");
                    }
                }
            }
            else {
                anim.Play("Fall");
            }
        }

        void _MovementHandler()
        {
            isGrounded = Physics2D.OverlapCircle(ground.position, 0.02f, groundMask);
            materialRay = Physics2D.CircleCast(feet.position, 0.02f, Vector2.down, 1.0f, footstepMask);

            velocity.x = input.x * moveForce; 
            velocity.y = rigid.velocity.y;
            velocity.y = Mathf.Clamp(velocity.y, -maxVelocityY, maxVelocityY);

            rigid.velocity = velocity;
        }

        void _FootStepHandler()
        {
            if (isGrounded) {

                if (rigid.velocity.y == 0.0f) {
                    
                    if (isFalling && materialRay) {
                        footStepAudioPlayer.PlayImpactForce(materialRay.transform.tag);
                        isFalling = false;
                    }

                    if (Input.GetAxisRaw("Horizontal") != 0.0f && materialRay) {
                        footStepAudioPlayer.Play(materialRay.transform.tag);
                    }
                }
            }
        }

        void _JumpHandler()
        {
            if (isGrounded && isPressedJump) {
                rigid.velocity = Vector2.up * jumpForce;
                audioSource.Play();
                isPressedJump = false;
            }

            if (rigid.velocity.y < 0.0f) {
                rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rigid.velocity.y > 0.0f && !Input.GetButton("Jump")) {
                rigid.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        void _FlipXHandler()
        {
            if (input.x > 0.0f) {
                newScale.x = 1.0f;
                transform.localScale = newScale;

            } else if (input.x < 0.0f) {
                newScale.x = -1.0f;
                transform.localScale = newScale;

            }
        }

        //Temp
        void _ResetPosition()
        {
            if (transform.position.y < -50.0f) {
                var newPos = transform.position;
                newPos.x = 0.0f;
                newPos.y = 6.0f;

                rigid.velocity = Vector2.zero;
                transform.position = newPos;
            }
        }
    }
}
