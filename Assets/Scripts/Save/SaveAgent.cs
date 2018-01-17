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
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;


        Collider2D hit;


        void Update()
        {
            if (hit) {
                _InputHandler();
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
            }
        }

        void _SaveGame()
        {
            SaveInstance.FireEvent_OnSave();
        }
    }
}
