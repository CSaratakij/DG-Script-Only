using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        int targetSceneIndex;

        [SerializeField]
        LayerMask allowEnterMask;

        [SerializeField]
        TargetType targetType;


        public enum TargetType
        {
            Door,
            Scene
        }

        public bool IsOpen { get { return isOpen; } set { isOpen = value; } }
        public bool IsAllowEnter { get { return isAllowEnter; } set { isAllowEnter = value; } }

        public Transform TargetDoor { get { return targetDoor; } }
        public int TargetScene { get { return targetSceneIndex; } }

        public bool IsUseTargetDoor { get { return targetType == TargetType.Door; } }
        public bool IsUseTargetScene { get { return targetType == TargetType.Scene; } }


        protected bool isOpen;


        bool isUseAxisY;

        Animator anim;
        Collider2D hit;


#if UNITY_EDITOR

         void OnDrawGizmos() {
            if (targetType == TargetType.Scene) {
                Gizmos.color = Color.blue;
                Gizmos.DrawWireCube(transform.position, new Vector3(1, 2, 1));
             }
         }

         void OnDrawGizmosSelected() {

             if (targetDoor && targetType == TargetType.Door) {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(targetDoor.position, new Vector3(1, 2, 1));
                Gizmos.DrawLine(transform.position, targetDoor.position);
                
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireCube(transform.position + offset, size);

                Handles.Label(targetDoor.position, "Target Door"); 
                Handles.Label(transform.position + offset, "Trigger Area"); 
             }

            if (targetType == TargetType.Scene) {

                var sceneName = SceneUtility.GetScenePathByBuildIndex(targetSceneIndex);

                if (sceneName != "") {
                    var label = string.Format("[ Go to Scene : {0} ]", sceneName);
                    Handles.Label(transform.position, label); 
                }
                else {
                    var label = string.Format("[ No scene index : {0} in Build Setting ]", targetSceneIndex);
                    Handles.Label(transform.position, label);
                }
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

                if (obj.gameObject.tag == "Player") {

                    var playerController = obj.gameObject.GetComponent<PlayerController>();
                    var worldWrappingController = obj.gameObject.GetComponent<WorldWrappingController>();

                    if (worldWrappingController && playerController) {

                        var isCanEnter = !(worldWrappingController.IsInEditMode || worldWrappingController.IsInMoveMode);

                        if (isCanEnter) {
                            playerController.StopUsingFocus();

                            _TargetDoor_Handler(obj);
                            _TargetScene_Handler(obj);
                        }
                    }
                }
                else {
                    _TargetDoor_Handler(obj);
                    _TargetScene_Handler(obj);
                }
            }
            else {
                Debug.Log("The door is locked..");
            }
        }

        void _TargetDoor_Handler(Transform obj)
        {
            if (targetDoor && targetType == TargetType.Door) {
                obj.position = targetDoor.position;
            }
            else {
                Debug.Log("Can't find target door..");
            }
        }

        void _TargetScene_Handler(Transform obj)
        {
            //todo
            if (targetType == TargetType.Scene) {
                //check if can get scene firt..

                //need to access GameController obj instance via sigleton
                //and use MoveToScene(targetScene.name);
                Debug.Log("About to move scene via Game Controller...");
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
