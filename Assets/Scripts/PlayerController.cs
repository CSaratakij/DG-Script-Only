using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class PlayerController : MonoBehaviour
    {
        const float HOVER_TIMER = 0.04f;

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

        [SerializeField]
        LayerMask footstepMask;

        [SerializeField]
        AudioClip jumpSound;


        bool isCanJump;
        bool isPressedJump;

        bool isGrounded;
        bool isFlipX;

        bool isHover;
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

        float hoverTimer;


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
            _InputHandler();
            _AnimationHandler();
            _FootStepHandler();

            //Temp
            _ResetPosition();
        }

        void FixedUpdate()
        {
            _MovementHandler();
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
            hoverTimer = HOVER_TIMER;
        }

        void _InputHandler()
        {
            input.x = Input.GetAxisRaw("Horizontal");

            if (isInverseMovement) {
                input.x *= -1.0f;
            }

            if (Input.GetButtonDown("Jump")) {
                isCanJump = true;
                isPressedJump = true;
                isFalling = false;
                hoverTimer = HOVER_TIMER;
                
                if (isGrounded && isCanJump) {
                    velocity.y = jumpForce;
                    rigid.velocity = velocity;
                    audioSource.Play();
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
            velocity.y -= gravity * Time.deltaTime;
            rigid.velocity = velocity;
        }

        void _FootStepHandler()
        {
            if (isGrounded) {
                cameraFollow.ForceFollowVertical();

                if (isPressedJump) {
                    if (isFalling && materialRay) {
                        footStepAudioPlayer.PlayImpact(materialRay.transform.tag);
                        isPressedJump = false;
                    }
                }
                else {
                    if (isHover && materialRay) {
                        footStepAudioPlayer.PlayImpact(materialRay.transform.tag);
                    }

                    isHover = false;
                    isFalling = false;
                    hoverTimer = HOVER_TIMER;
                }

                if (Input.GetAxisRaw("Horizontal") != 0.0f && materialRay) {
                    footStepAudioPlayer.Play(materialRay.transform.tag);
                }
            }
            else {
                isHover = true;
                hoverTimer -= 0.1f * Time.deltaTime;

                if (hoverTimer <= 0.0f) {
                    isFalling = true;
                }

                cameraFollow.UnForceFollowVertical();
            }
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

        //Temp
        void _ResetPosition()
        {
            if (transform.position.y < -5.0f) {
                var newPos = transform.position;
                newPos.x = 0.0f;
                newPos.y = 6.0f;
                transform.position = newPos;
            }
        }
    }
}
