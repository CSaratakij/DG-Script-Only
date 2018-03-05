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


        int hitCount;
        Collider2D[] hit;


#if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, size);
            Handles.Label(transform.position, "Trigger Area");
        }
#endif

        void Awake()
        {
            hit = new Collider2D[3];
        }

        void FixedUpdate()
        {
            hitCount = Physics2D.OverlapBoxNonAlloc(transform.position + offset, size, 0.0f, hit, itemMask);

            if (hitCount <= 0) {
                return;
            }

            foreach (Collider2D collider in hit) {

                if (!collider) {
                    continue;
                }

                var item = collider.transform.gameObject.GetComponent<Item>();

                if (!item) {
                    continue;
                }

                if (item.IsUsed) {
                    continue;
                }

                item.Collect();
            }
        }
    }
}
