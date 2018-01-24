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
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;

        [SerializeField]
        GameObject uiObject;


        public bool IsUsed { get { return isUsed; } set { isUsed = value; } }


        bool isUsed;

        Collider2D hit;
        GameObject target;


#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireCube(transform.position, size);

            Handles.Label(transform.position, "Trigger Area");

            if (unlockList.Length > 0) {
                Handles.Label(transform.position + Vector3.down * 0.2f, "Unlock : \n" + _GetAllUnlockName());
            }
            else {
                Handles.Label(transform.position + Vector3.down * 0.2f, "None 'Focus' ability to unlock.");
            }
        }
#endif

        void Start()
        {
            gameObject.SetActive(!isUsed);
        }

        void Update()
        {
            if (hit) {
                if (isUsed) {
                    _ToggleInteractUI(false);
                }
                else {
                    _ToggleInteractUI(true);
                    _InputHandler();
                }
            }
            else {
                _ToggleInteractUI(false);
            }
        }

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position, size, 0.0f, layerMask);
        }

        void _InputHandler()
        {
            if (Input.GetButtonDown("Interact")) {
                _UnlockFocus();
            }
        }

        void _UnlockFocus()
        {
            var worldWrappingControl = hit.gameObject.GetComponent<WorldWrappingController>();

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
            gameObject.SetActive(false);
            // and its save agent -> save its used state?
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

        void _ToggleInteractUI(bool value) {
            if (uiObject) {
                if (uiObject.activeSelf != value) {
                    uiObject.SetActive(value);
                }
            }
        }
    }
}
