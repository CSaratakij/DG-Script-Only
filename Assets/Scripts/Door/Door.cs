using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

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


        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
        public bool IsAllowEnter { get { return isAllowEnter; } set { isAllowEnter = value; } }

        public Transform TargetDoor { get { return targetDoor; } }

        protected bool isOpen;


        bool isUseAxisY;

        Animator anim;
        Collider2D hit;


#if UNITY_EDITOR
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
#endif

         void _InputHandler()
         {
             var axisY = Input.GetAxisRaw("Vertical");

             if (axisY > 0.0f && !isUseAxisY) {
                 if (isAllowEnter && hit) {
                     this.Enter(hit.transform);
                 }

                 isUseAxisY = true;
             }

             if (axisY == 0.0f) {
                 isUseAxisY = false;
             }
         }

         void _AnimationHandler()
         {
             if (isOpen) {
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

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
        }

        protected virtual void Update()
        {
            _InputHandler();
            _AnimationHandler();
        }

        protected virtual void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position + offset, size, 0.0f, allowEnterMask);
        }

        protected virtual void Enter(Transform obj)
        {
            if (isAllowEnter) {

                if (targetDoor) {

                    if (obj.gameObject.tag == "Player") {
                        var playerController = obj.gameObject.GetComponent<PlayerController>();
                        playerController.StopUsingFocus();
                    }

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
                isOpen = true;
            }
        }

        public void Close()
        {
            if (isOpen) {
                isOpen = false;
            }
        }
    }
}
