using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class BoxController : MonoBehaviour
    {
        [SerializeField]
        bool isEffectByFocus;

        [SerializeField]
        float moveForce;

        [SerializeField]
        float resetDelay;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;

        [SerializeField]
        LayerMask resetterMask;

        [SerializeField]
        GameObject uiObject;


        int hitPlayerCount;
        int hitBoxResetterCount;

        bool isInitUsing;
        bool isInitReset;

        bool isUsing;
        bool isInteractable;

        Vector2 inputVector;
        Vector2 velocity;

        Vector3 originalPosition;

        Collider2D[] hit;
        Collider2D[] hitBoxResetter;

        Rigidbody2D rigid;
        PlayerController playerControl;

        WorldWrappingController worldWrappingControl;
        FocusEffector focusEffector;

        PlatformAttacher platformAttacher;

#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, size);
            Handles.Label(transform.position, "Trigger Area");
        }
#endif

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
            focusEffector = GetComponent<FocusEffector>();
            platformAttacher = GetComponent<PlatformAttacher>();

            hit = new Collider2D[1];
            hitBoxResetter = new Collider2D[1];
        }

        void Start()
        {
            originalPosition = transform.position;
        }

        void Update()
        {
            if (platformAttacher) {
                platformAttacher.Use(!isUsing && !isInitReset);
            }

            _InputHandler();
            _CheckInteractable();
            _ToggleUIHandler();
        }

        void FixedUpdate()
        {
            hitPlayerCount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0.0f,  hit, layerMask);
            hitBoxResetterCount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0.0f,  hitBoxResetter, resetterMask);

            if (isUsing) {

                if (hitPlayerCount > 0) {

                    velocity.x = inputVector.x * moveForce;
                    velocity.y = rigid.velocity.y;

                    rigid.velocity = velocity;
                }
                else {
                    isUsing = false;
                }
            }
            else {
                var newVelocity = rigid.velocity;

                newVelocity.x = 0.0f;
                newVelocity.y = Mathf.Clamp(newVelocity.y, -8.0f, 8.0f);

                rigid.velocity = newVelocity;
            }

            if (hitBoxResetterCount > 0) {
                if (!isInitReset) {
                    StartCoroutine(_Reset_Box_CallBack());
                    isInitReset = true;
                }
            }
        }

        void _InputHandler()
        {
            if (isUsing) {

                if (Input.GetButtonDown("Interact")) {

                    if (playerControl) {
                        playerControl.IsUsingBox = false;
                        playerControl.AvatarDirFromBox = Vector2.zero;
                    }

                    isUsing = false;
                    isInitUsing = false;
                }

                if (playerControl) {

                    var axisX = Input.GetAxisRaw("Horizontal");

                    //Hacks
                    if (worldWrappingControl) {

                        if (worldWrappingControl.IsInEditMode || worldWrappingControl.IsInMoveMode) {
                            axisX = 0.0f;
                        }
                    }
                    else {
                        axisX = 0.0f;
                    }

                    if (axisX > 0.0f) {
                        inputVector.x = 1.0f;
                    }
                    else if (axisX < 0.0f) {
                        inputVector.x = -1.0f;
                    }
                    else {
                        inputVector.x = 0.0f;
                    }
                }
            }
            else {
                if (hitPlayerCount > 0) {

                    if (!isInteractable) {
                        return;
                    }

                    if (Input.GetButtonDown("Interact")) {

                        isInitUsing = true;
                        isUsing = true;

                        playerControl = hit[0].GetComponent<PlayerController>();

                        if (playerControl) {
                            worldWrappingControl = playerControl.GetComponent<WorldWrappingController>();
                        }

                        if (playerControl && !playerControl.IsUsingBox) {
                            playerControl.IsUsingBox = true;

                            if (playerControl.gameObject.transform.position.x > transform.position.x) {
                                playerControl.AvatarDirFromBox = Vector2.right;
                            }
                            else if (playerControl.gameObject.transform.position.x < transform.position.x) {
                                playerControl.AvatarDirFromBox = Vector2.left;
                            }
                        }

                        if (isEffectByFocus && focusEffector) {
                            focusEffector.SetAffector(playerControl);
                            focusEffector.UseEffector(true);
                        }
                    }
                }
                else {
                    if (playerControl) {

                        if (isInitUsing) {

                            if (!isUsing) {
                                playerControl.IsUsingBox = false;
                                playerControl.AvatarDirFromBox = Vector2.zero;
                            }

                            isInitUsing = false;
                        }

                        if (isEffectByFocus) {
                            if (worldWrappingControl) {

                                if (!worldWrappingControl.IsUseFocus) {

                                    playerControl = null;
                                    worldWrappingControl = null;

                                    focusEffector.SetAffector(null);
                                    focusEffector.UseEffector(false);
                                }
                            }
                        }
                        else {
                            playerControl = null;
                            worldWrappingControl = null;
                        }
                    }
                }
            }
        }

        void _CheckInteractable()
        {
            if (hitPlayerCount <= 0) {
                isInteractable = false;
                return;
            }

            if (isUsing) {
                isInteractable = true;
            }
            else {
                if (hit[0].transform.position.x < transform.position.x) {
                    isInteractable = (hit[0].transform.localScale.x > 0.0f);
                }
                else if (hit[0].transform.position.x > transform.position.x) {
                    isInteractable = (hit[0].transform.localScale.x < 0.0f);
                }
                else {
                    isInteractable = false;
                }
            }
        }

        void _ToggleUIHandler()
        {
            if (hitPlayerCount > 0) {
                if (isUsing) {
                    _ToggleInteractUI(false);
                }
                else {
                    _ToggleInteractUI(isInteractable);
                }
            }
            else {
                _ToggleInteractUI(false);
            }
        }

        void _ToggleInteractUI(bool value)
        {
            if (uiObject) {
                if (uiObject.activeSelf != value) {
                    uiObject.SetActive(value);
                }
            }
            else {
                Debug.Log("Can't find ui interact object..");
            }
        }

        IEnumerator _Reset_Box_CallBack()
        {
            yield return new WaitForSeconds(resetDelay);
            transform.position = originalPosition;

            yield return new WaitForSeconds(0.3f);
            isInitReset = false;
        }
    }
}
