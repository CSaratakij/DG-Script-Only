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
        Transform targetDoor;

        public bool IsOpen { get { return isOpen; } }
        public Transform TargetDoor { get { return targetDoor; } }

        protected bool isOpen;


        Animator anim;


         void OnDrawGizmosSelected() {
             if (targetDoor) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(targetDoor.position, new Vector3(1, 1, 1));
                Gizmos.DrawLine(transform.position, targetDoor.position);
                Handles.Label(targetDoor.position, "Target Door"); 
             }
        }

        protected virtual void Awake()
        {
            anim = GetComponent<Animator>();
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
