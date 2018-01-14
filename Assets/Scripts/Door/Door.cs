using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DG
{
    [RequireComponent(typeof(Animator))]
    public class Door : MonoBehaviour
    {
        [SerializeField]
        protected bool isAllowEnter;

        [SerializeField]
        Vector3 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        Transform targetDoor;

        [SerializeField]
        LayerMask allowEnterMask;


        public bool IsOpen { get { return isOpen; } }
        public bool IsAllowEnter { get { return isAllowEnter; } }

        public Transform TargetDoor { get { return targetDoor; } }

        protected bool isOpen;


        Animator anim;
        Collider2D hit;


         void OnDrawGizmosSelected() {
             if (targetDoor) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(targetDoor.position, new Vector3(1, 1, 1));
                Gizmos.DrawLine(transform.position, targetDoor.position);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position + offset, size);

                Handles.Label(targetDoor.position, "Target Door"); 
                Handles.Label(transform.position + offset, "Trigger Area"); 
             }
        }

         void _InputHandler()
         {
             if (Input.GetButtonDown("Interact") && isAllowEnter && hit) {
                 this.Enter(hit.transform);
             }
         }

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            _InputHandler();
        }

        protected virtual void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position + offset, size, 0.0f, allowEnterMask);
        }

        protected virtual void Enter(Transform obj)
        {
            if (isAllowEnter) {
                if (targetDoor) {
                    obj.position = targetDoor.position;
                }
                else {
                    Debug.Log("Can't find target door..");
                }
            }
            else {
                Debug.Log("The door is locked..");
            }
        }

        public void Open()
        {
            if (!isOpen) {
                anim.Play("Open");
                isOpen = true;
            }
        }

        public void Close()
        {
            if (isOpen) {
                anim.Play("Close");
                isOpen = false;
            }
        }
    }
}
