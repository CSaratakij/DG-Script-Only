using UnityEngine;

namespace DG
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        bool isMain;

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


        public static bool isInCinematic = false;


        bool isControlable;
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

        WorldWrappingController worldWrappingControl;
        SpriteRenderer render;


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

            /*
            if (worldWrappingControl) {
                worldWrappingControl.AllowFocus(true);
            }
            */

            if (render) {
                render.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        void Update()
        {
            _InputHandler();

            if (rigid.velocity.y < 0.0f) {
                isFalling = true;
            }

            if (isControlable) {
                _FlipXHandler();
            }

            _AnimationHandler();
            _FootStepHandler();
            _FocusHandler();

            if (isInCinematic) {
                StopUsingFocus();
                _Controlable(false);
            }
            else {
                if (!worldWrappingControl.IsUseFocus && !isControlable) {
                    _Controlable(true);
                }
            }

            //Temp
            /* _ResetPosition(); */
        }

        void FixedUpdate()
        {
            isGrounded = Physics2D.OverlapCircle(ground.position, 0.02f, groundMask);
            materialRay = Physics2D.CircleCast(feet.position, 0.02f, Vector2.down, 1.0f, footstepMask);

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
            worldWrappingControl = GetComponent<WorldWrappingController>();
            render = GetComponent<SpriteRenderer>();
            isControlable = true;
        }

        void _InputHandler()
        {
            input.x = Input.GetAxisRaw("Horizontal");

            if (isInverseMovement) {
                input.x *= -1.0f;
            }

            if (isControlable) {
                if (Input.GetButtonDown("Jump")) {
                    isPressedJump = true;
                }
            }

            if (worldWrappingControl.IsCanFocus) {

                if (worldWrappingControl.IsUseFocus) {

                    if (Input.GetButtonDown("Focus")) {

                        if (worldWrappingControl.IsCanEditMode && worldWrappingControl.IsInEditMode) {
                            _ToggleEditMode();
                        }
                        else {
                            _ToggleFocus();
                        }
                    }

                    var isUseMoveMode = Input.GetButton("MoveMode");
                    var isUseMoveModeByAxis = Input.GetAxisRaw("MoveMode");

                    if (isUseMoveMode || isUseMoveModeByAxis == 1.0f) {
                        
                        if (worldWrappingControl.IsCanMoveMode) {
                            worldWrappingControl.UseMoveMode(true);
                            render.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                            _Controlable(false);
                        }
                        else {
                            if (worldWrappingControl.IsInEditMode) {
                                render.maskInteraction = SpriteMaskInteraction.None;
                            }

                            _Controlable(!worldWrappingControl.IsInEditMode);
                            worldWrappingControl.UseMoveMode(false);
                        }
                    }
                    else {
                        if (worldWrappingControl.IsInEditMode) {
                            render.maskInteraction = SpriteMaskInteraction.None;
                        }

                        _Controlable(!worldWrappingControl.IsInEditMode);
                        worldWrappingControl.UseMoveMode(false);
                    }
                }
                else {
                    if (isGrounded && Input.GetButtonDown("Focus")) {
                        _ToggleFocus();
                    }
                }
            }
            else {
                if (worldWrappingControl.IsUseFocus) {
                    StopUsingFocus();
                }
            }
        }

        void _AnimationHandler()
        {
            if (isControlable) {
                if (isGrounded) {
                    if (input.x != 0.0f) {
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
            else {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                    anim.Play("Falling Impact");
                }
            }
        }

        void _MovementHandler()
        {
            if (isControlable) {

                velocity.x = input.x * moveForce; 
                velocity.y = rigid.velocity.y;

                velocity.y = Mathf.Clamp(velocity.y, -maxVelocityY, maxVelocityY);
                rigid.velocity = velocity;
            }
            else {

                velocity = Vector2.zero;

                velocity.y = rigid.velocity.y;
                velocity.y = Mathf.Clamp(velocity.y, -maxVelocityY, maxVelocityY);

                rigid.velocity = velocity;
            }
        }

        void _FootStepHandler()
        {
            if (isGrounded) {

                if (rigid.velocity.y == 0.0f) {
                    
                    if (isFalling && materialRay) {
                        footStepAudioPlayer.PlayImpactForce(materialRay.transform.tag);
                        isFalling = false;
                    }

                    if (isControlable) {
                        if (input.x != 0.0f && materialRay) {
                            footStepAudioPlayer.Play(materialRay.transform.tag);
                        }
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

        void _ToggleFocus()
        {
            var isUseFocus = !worldWrappingControl.IsUseFocus;

            if (isUseFocus) {
                render.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
                cameraFollow.LockCamera();
            }
            else {
                render.maskInteraction = SpriteMaskInteraction.None;
                cameraFollow.UnlockCamera();
            }

            worldWrappingControl.UseFocus(isUseFocus);
        }

        void _FocusHandler()
        {
            if (worldWrappingControl.IsCanFocus && worldWrappingControl.IsUseFocus) {
                if (worldWrappingControl.IsCanEditMode) {
                    if (Input.GetButtonDown("FocusEditMode")) {
                        _ToggleEditMode();
                    }
                }
                else {
                    worldWrappingControl.UseEditMode(false);
                }
            }
            else {
                worldWrappingControl.UseEditMode(false);
                _Controlable(true);
                render.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        void _ToggleEditMode()
        {
            var isInEditMode = worldWrappingControl.IsInEditMode;

            if (isInEditMode) {
                worldWrappingControl.UseEditMode(false);
                _Controlable(true);
                render.maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
            }
            else {
                worldWrappingControl.UseEditMode(true);
                _Controlable(false);
                render.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        void _Controlable(bool value) {
            isControlable = value;
        }

        //Temp
        /*
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
        */

        public void StopUsingFocus()
        {
            worldWrappingControl.UseFocus(false);
            cameraFollow.UnlockCamera();
        }
    }
}
