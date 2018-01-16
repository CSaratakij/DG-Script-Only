using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DG
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Switch))]
    public class ToggleSwitch : MonoBehaviour
    {
        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;


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

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position, size);
            Handles.Label(transform.position, "Trigger Area");
        }

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
