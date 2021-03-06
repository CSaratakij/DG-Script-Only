﻿using UnityEngine;

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
        LayerMask wallMask;

        [SerializeField]
        LayerMask footstepMask;

        [SerializeField]
        AudioClip jumpSound;


        public static bool isInCinematic = false;

        public bool IsUsingBox { get { return isUsingBox; } set { isUsingBox = value; } }
        public Vector2 AvatarDirFromBox { get { return avatarDirFromBox; } set { avatarDirFromBox = value; } }


        int groundHitCount;
        int wallHitCount;
        int materialHitCount;

        bool isUsingBox;
        bool isControlable;

        bool isGrounded;
        bool isWallJump;
        bool isFalling;

        Collider2D[] groundHit;

        RaycastHit2D[] wallHit;
        RaycastHit2D[] wallLeftHit;

        Vector2 input;
        Vector2 velocity;
        Vector2 avatarDirFromBox;

        Animator anim;
        Rigidbody2D rigid;

        AudioSource audioSource;
        FootStepAudioPlayer footStepAudioPlayer;

        Transform ground;
        Transform feet;
        Transform wallRight;

        RaycastHit2D[] materialRay;
        CameraFolllow cameraFollow;

        Vector3 newScale;

        WorldWrappingController worldWrappingControl;
        SpriteRenderer render;

        PlatformAttacher platformAttacher;


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

            if (render) {
                render.maskInteraction = SpriteMaskInteraction.None;
            }
        }

        void Update()
        {
            _InputHandler();

            if (rigid.velocity.y < -5.0f) {
                isFalling = true;
            }

            if (isControlable && !isUsingBox && !isWallJump) {
                _FlipXHandler();
            }

            _AnimationHandler();
            _FootStepHandler();
            _FocusHandler();

            if (isInCinematic) {
                StopUsingFocus();
                _Controlable(false);
                Debug.Log("In cinematic..");
            }
            else {
                if (!worldWrappingControl.IsUseFocus && !isControlable) {
                    _Controlable(true);
                }
            }

            //Hacks
            if (platformAttacher) {

                if (rigid.velocity.y <= 0.3f) {
                    if (isGrounded) {
                        platformAttacher.Use(input.x == 0.0f);
                    }
                }
                else {
                    platformAttacher.Use(false);
                }
            }

            //Hacks
            _WallJumpHandler();

            if (isWallJump) {
                if (rigid.velocity.y < 0.0f) {
                    rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
                }
            }
            else {
                _JumpHandler();
                _MovementHandler();
            }
        }

        void FixedUpdate()
        {
            groundHitCount = Physics2D.OverlapCircleNonAlloc(ground.position, 0.02f, groundHit, groundMask);
            isGrounded = groundHitCount > 0;

            if (isGrounded) {
                isWallJump = false;
            }

            materialHitCount = Physics2D.CircleCastNonAlloc(feet.position, 0.02f, Vector2.down, materialRay, 1.0f, footstepMask);
            wallHitCount = Physics2D.RaycastNonAlloc(wallRight.position, Vector2.right, wallHit, 0.3f, wallMask);
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
            groundHit = new Collider2D[1];
            wallHit = new RaycastHit2D[1];
            ground = transform.Find("ground");
            feet = transform.Find("footstep");
            wallRight = transform.Find("wallRight");
            footStepAudioPlayer = transform.Find("footstep").gameObject.GetComponent<FootStepAudioPlayer>();
            cameraFollow = Camera.main.GetComponent<CameraFolllow>();
            newScale = transform.localScale;
            worldWrappingControl = GetComponent<WorldWrappingController>();
            render = GetComponent<SpriteRenderer>();
            isControlable = true;
            platformAttacher = GetComponent<PlatformAttacher>();
            materialRay = new RaycastHit2D[1];
        }

        void _InputHandler()
        {
            input.x = Input.GetAxisRaw("Horizontal");

            if (isInverseMovement) {
                input.x *= -1.0f;
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
                            worldWrappingControl.UseMoveMode(isGrounded);

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
                if (isUsingBox) {

                    if (avatarDirFromBox.x > 0.0f) {

                        if (transform.localScale.x > 0.0f) {
                            var newScale = transform.localScale;
                            newScale.x = -1.0f;
                            transform.localScale = newScale;
                        }

                        if (input.x > 0.0f) {
                            anim.Play("Pull");
                        }
                        else if (input.x < 0.0f) {
                            anim.Play("Push");
                        }
                        else {
                            anim.Play("Pull Idle");
                        }
                    }
                    else if (avatarDirFromBox.x < 0.0f) {

                        if (transform.localScale.x < 0.0f) {
                            var newScale = transform.localScale;
                            newScale.x = 1.0f;
                            transform.localScale = newScale;
                        }

                        if (input.x > 0.0f) {
                            anim.Play("Push");
                        }
                        else if (input.x < 0.0f) {
                            anim.Play("Pull");
                        }
                        else {
                            anim.Play("Pull Idle");
                        }
                    }
                }
                else {
                    if (isGrounded) {
                        //Hacks
                        if (Input.GetButton("Jump") && rigid.velocity.y > 1.0f) {
                            anim.Play("Fall");
                        }
                        else {
                            if (input.x != 0.0f) {
                                anim.Play("Run");
                            }
                            else {
                                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Idle")) {
                                    anim.Play("Falling Impact");
                                }
                            }
                        }
                    }
                    else {
                        //Test
                        if (platformAttacher.IsUse) {
                            anim.Play("Idle");
                        }
                        else {
                            anim.Play("Fall");
                        }
                    }
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

                if (isFalling && materialHitCount > 0) {

                    if (!platformAttacher.IsUse) {
                        footStepAudioPlayer.PlayImpactForce(materialRay[0].transform.tag);
                    }

                    isFalling = false;
                }

                if (isControlable) {
                    if (input.x != 0.0f && materialHitCount > 0) {
                        footStepAudioPlayer.Play(materialRay[0].transform.tag);
                    }
                    else {
                        footStepAudioPlayer.StopFootStep();
                    }
                }
            }
        }

        void _JumpHandler()
        {
            if (!isControlable) { return; }
            if (isGrounded && Input.GetButtonDown("Jump")) {
                rigid.velocity = Vector2.up * jumpForce;
                audioSource.Play();
            }

            if (rigid.velocity.y < 0.0f) {
                rigid.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
            }
            else if (rigid.velocity.y > 0.0f && !Input.GetButton("Jump")) {
                rigid.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.deltaTime;
            }
        }

        void _WallJumpHandler()
        {
            if (!isControlable) { return; }
            if (isGrounded) { return; }
            if (wallHitCount <= 0) { return; }

            if (Input.GetButtonDown("Jump")) {

                isWallJump = true;
                audioSource.Play();

                if (transform.position.x < wallHit[0].transform.position.x) {
                    var direction = new Vector2(-0.3f, 0.7f);

                    var newScale = transform.localScale;
                    newScale.x = -1;

                    transform.localScale = newScale;

                    var force = 12.0f;
                    rigid.velocity = direction * force;
                }
                else if (transform.position.x > wallHit[0].transform.position.x) {
                    var direction = new Vector2(0.3f, 0.7f);

                    var newScale = transform.localScale;
                    newScale.x = 1;

                    transform.localScale = newScale;

                    var force = 12.0f;
                    rigid.velocity = direction * force;
                }
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

        public void StopUsingFocus()
        {
            worldWrappingControl.UseFocus(false);
            cameraFollow.UnlockCamera();
        }
    }
}
