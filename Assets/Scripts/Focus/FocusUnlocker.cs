using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class FocusUnlocker : MonoBehaviour
    {
        public enum UnlockType
        {
            Focus,
            EditMode,
            MoveMode
        }

        [SerializeField]
        UnlockType[] unlockList;

        [SerializeField]
        Vector3 origin;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;


        public bool IsUsed { get { return isUsed; } set { isUsed = value; } }


        int hitCount;
        bool isUsed;

        Animator anim;

        Collider2D[] hit;
        GameObject target;
        


#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(origin + transform.position, size);

            Handles.Label(transform.position, "Trigger Area");

            if (unlockList.Length > 0) {
                Handles.Label(origin + transform.position + (Vector3.down * 0.2f), "Unlock : \n" + _GetAllUnlockName());
            }
            else {
                Handles.Label(origin + transform.position + (Vector3.down * 0.2f), "None 'Focus' ability to unlock.");
            }
        }
#endif

        void Awake()
        {
            hit = new Collider2D[1];
            anim = GetComponent<Animator>();
        }

        void Update()
        {
            _AnimationHandler();

            if (isUsed) { return; }
            if (hitCount > 0) {
                _UnlockFocus();
            }
        }

        void FixedUpdate()
        {
            if (isUsed) { return; }
            hitCount = Physics2D.OverlapBoxNonAlloc(origin + transform.position, size, 0.0f,  hit, layerMask);
        }

        void _AnimationHandler()
        {
            anim.SetBool("isUsed", isUsed);
        }

        void _UnlockFocus()
        {
            var worldWrappingControl = hit[0].gameObject.GetComponent<WorldWrappingController>();

            if (worldWrappingControl) {
                foreach (var unlockType in unlockList) {
                    switch (unlockType) {
                        case UnlockType.Focus:
                            worldWrappingControl.IsCanFocus = true;
                            break;

                        case UnlockType.MoveMode:
                            worldWrappingControl.IsCanMoveMode = true;
                            break;

                        case UnlockType.EditMode:
                            worldWrappingControl.IsCanEditMode = true;
                            break;
                    }
                }

                _Disabled();
            }
            else {
                Debug.Log("Can't unlock focus ability..");
            }
        }

        void _Disabled()
        {
            isUsed = true;
        }

        string _GetAllUnlockName()
        {
            var result = "";
            foreach (var unlockType in unlockList) {
                switch (unlockType) {
                    case UnlockType.Focus:
                        result += "\t[ Focus ]";
                        break;

                    case UnlockType.MoveMode:
                        result += "\t[ MoveMode ]";
                        break;

                    case UnlockType.EditMode:
                        result += "\t[ EditMode ]";
                        break;
                }
                result += "\n";
            }
            return result;
        }
    }
}
