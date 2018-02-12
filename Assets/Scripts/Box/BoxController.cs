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


        bool isInitUsing;
        bool isInitReset;

        bool isUsing;

        Vector2 inputVector;
        Vector2 velocity;

        Vector3 originalPosition;

        Collider2D hit;
        Collider2D hitBoxResetter;

        Rigidbody2D rigid;

        PlayerController playerControl;

        WorldWrappingController worldWrappingControl;
        FocusEffector focusEffector;


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
        }

        void Start()
        {
            originalPosition = transform.position;
        }

        void Update()
        {
            _InputHandler();
            _ToggleUIHandler();
        }

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position, size, 0.0f, layerMask);
            hitBoxResetter = Physics2D.OverlapBox(transform.position, size, 0.0f, resetterMask);

            if (isUsing) {

                if (hit) {

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

            if (hitBoxResetter) {
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
                    isUsing = false;

                    if (playerControl) {
                        playerControl.IsUsingBox = false;
                        playerControl.AvatarDirFromBox = Vector2.zero;
                    }
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
                if (hit) {
                    if (Input.GetButtonDown("Interact")) {

                        isInitUsing = true;
                        isUsing = true;

                        playerControl = hit.GetComponent<PlayerController>();

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

                        if (isEffectByFocus) {
                            focusEffector.SetAffector(playerControl);
                            focusEffector.UseEffector(true);
                        }
                    }
                }
                else {
                    if (playerControl) {

                        if (isInitUsing) {
                            playerControl.IsUsingBox = false;
                            playerControl.AvatarDirFromBox = Vector2.zero;

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

        void _ToggleUIHandler()
        {
            if (hit) {
                if (isUsing) {
                    _ToggleInteractUI(false);
                }
                else {
                    //check first if player is really look at a box..
                    _ToggleInteractUI(true);
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

        //check first if player is really look at a box..
        bool _IsInteractable()
        {
            var result = false;
            return result;
        }

        IEnumerator _Reset_Box_CallBack()
        {
            yield return new WaitForSeconds(resetDelay);
            rigid.position = originalPosition;
            isInitReset = false;
        }
    }
}
