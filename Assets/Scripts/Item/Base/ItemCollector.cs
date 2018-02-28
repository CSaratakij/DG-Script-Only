using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class ItemCollector : MonoBehaviour
    {
        [SerializeField]
        Vector3 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask itemMask;


        Collider2D hit;


#if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, size);
            Handles.Label(transform.position, "Trigger Area");
        }
#endif

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position + offset, size, 0.0f, itemMask);

            if (!hit) {
                return;
            }

            var item = hit.transform.gameObject.GetComponent<Item>();

            if (!item) {
                return;
            }

            if (item.IsUsed) {
                return;
            }

            item.Collect();
        }
    }
}
