using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class FocusDisableArea : MonoBehaviour
    {
        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask playerMask;


        int hitCount;
        bool isInitHit;

        WorldWrappingController worldWrappingControl;
        Collider2D[] hit;


#if UNITY_EDITOR
        void OnDrawGizmos() {
            var resultPosition = transform.position + new Vector3(offset.x, offset.y, 0.0f);

            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(resultPosition, size);

            Handles.Label(resultPosition, "Trigger Area");
        }
#endif

        void Awake()
        {
            worldWrappingControl = null;
            hit = new Collider2D[1];
        }

        void Update()
        {
            if (hitCount <= 0) {
                if (worldWrappingControl) {
                    worldWrappingControl.AllowFocus(true);
                }
                worldWrappingControl = null;
                return;
            }

            worldWrappingControl = hit[0].GetComponent<WorldWrappingController>();

            if (!worldWrappingControl) {
                return;
            }

            worldWrappingControl.AllowFocus(false);
        }

        void FixedUpdate()
        {
            hitCount = Physics2D.OverlapBoxNonAlloc(transform.position + new Vector3(offset.x, offset.y, 0.0f), size, 0.0f,  hit, playerMask);
        }
    }
}
