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
        float moveForce;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;

        [SerializeField]
        GameObject uiObject;


        bool isUsing;

        Vector2 inputVector;
        Vector2 velocity;

        Collider2D hit;
        Rigidbody2D rigid;

        PlayerController playerControl;


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
        }

        void Update()
        {
            _InputHandler();
            _ToggleUIHandler();
        }

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position, size, 0.0f, layerMask);

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
                rigid.velocity = newVelocity;
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

                        isUsing = true;
                        playerControl = hit.GetComponent<PlayerController>();

                        if (playerControl && !playerControl.IsUsingBox) {
                            playerControl.IsUsingBox = true;

                            if (playerControl.gameObject.transform.position.x > transform.position.x) {
                                playerControl.AvatarDirFromBox = Vector2.right;
                            }
                            else if (playerControl.gameObject.transform.position.x < transform.position.x) {
                                playerControl.AvatarDirFromBox = Vector2.left;
                            }
                        }
                    }
                }
                else {
                    if (playerControl) {
                        playerControl.IsUsingBox = false;
                        playerControl.AvatarDirFromBox = Vector2.zero;
                        playerControl = null;
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
    }
}
