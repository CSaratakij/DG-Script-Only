using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class SaveAgent : MonoBehaviour
    {
        [SerializeField]
        bool autoSave;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;

        [SerializeField]
        GameObject uiObject;


        Collider2D hit;


        bool isAlreadyEnter;


        void Update()
        {
            if (hit) {
                if (autoSave) {
                    if (!isAlreadyEnter) {
                        _SaveGame();

                        if (SaveNotification.instance) {
                            SaveNotification.instance.ShowNotification();
                        }

                        isAlreadyEnter = true;
                    }
                }
                else {
                    _InputHandler();

                    if (!uiObject.activeSelf) {
                        uiObject.SetActive(true);
                    }
                }
            }
            else {
                if (uiObject.activeSelf) {
                    uiObject.SetActive(false);
                }

                isAlreadyEnter = false;
            }
        }

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position, size, 0.0f, layerMask);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position, size);
            Handles.Label(transform.position, "Trigger Area");
        }
#endif

        void _InputHandler()
        {
            if (Input.GetButtonDown("Interact")) {
                _SaveGame();

                if (SaveNotification.instance) {
                    SaveNotification.instance.ShowNotification();
                }
            }
        }

        void _SaveGame()
        {
            SaveInstance.FireEvent_OnSave();
        }
    }
}
