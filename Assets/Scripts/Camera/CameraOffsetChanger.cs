using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class CameraOffsetChanger : MonoBehaviour
    {
        [SerializeField]
        bool isResetAfterExit;

        [SerializeField]
        Vector2 cameraOffset;

        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask activatorMask;


        bool isInitHit;
        Vector2 initHitOffset;

        Collider2D hit;
        Object lastHitRef;

#if UNITY_EDITOR
        void OnDrawGizmos() {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireCube(transform.position + new Vector3(offset.x, offset.y, 0.0f), size);

            var textResult = string.Format("Offset : ({0}, {1})", cameraOffset.x, cameraOffset.y);
            Handles.Label(transform.position, textResult);
        }
#endif

        void Awake()
        {
            lastHitRef = null;
        }

        void Update()
        {
            if (isInitHit && !hit && lastHitRef) {

                if (isResetAfterExit) {
                    _ResetCameraOffset();
                }

                lastHitRef = null;
                isInitHit = false;
            }
        }

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position + new Vector3(offset.x, offset.y, 0.0f), size, 0.0f, activatorMask);

            if (hit) {
                
                if (!isInitHit) {

                    var cameraFollow = Camera.main.GetComponent<CameraFolllow>();
                    initHitOffset = new Vector2(cameraFollow.OffsetX, cameraFollow.OffsetY);

                    _SetCameraOffset(cameraOffset);
                    isInitHit = true;
                }

                lastHitRef = hit;
            }
        }

        void _SetCameraOffset(Vector2 offset)
        {
            if (!Camera.main) {
                return;
            }

            var cameraFollow = Camera.main.GetComponent<CameraFolllow>();

            if (!cameraFollow) {
                return;
            }

            cameraFollow.SetOffsetX(offset.x);
            cameraFollow.SetOffsetY(offset.y);
        }

        void _ResetCameraOffset()
        {
            _SetCameraOffset(initHitOffset);
        }
    }
}
