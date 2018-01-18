using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Switch))]
    public class ToggleSwitch : MonoBehaviour
    {
        [SerializeField]
        LayerMask layerMask;

        [SerializeField]
        Vector2 size;


        Animator anim;
        Switch switchObj;

        Collider2D hit;


        void Awake()
        {
            anim = GetComponent<Animator>();
            switchObj = GetComponent<Switch>();
        }

        void Update()
        {
            if (hit) {
                _InputHandler();
            }

            _AnimationHandler();
        }

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position, size, 0.0f, layerMask);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, size);
            Handles.Label(transform.position, "Trigger Area");
        }
#endif

        void _InputHandler()
        {
            if (Input.GetButtonDown("Interact")) {
                _ToggleOpen();
            }
        }

        void _AnimationHandler()
        {
            if (switchObj.IsTurnOn) {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Open")) {
                    anim.Play("Open");
                }
            }
            else {
                if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Close")) {
                    anim.Play("Close");
                }
            }
        }

        void _ToggleOpen()
        {
            if (switchObj.IsTurnOn) {
                switchObj.TurnOff();
            }
            else {
                switchObj.TurnOn();
            }
        }
    }
}
