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
        bool isResetByPreviousOffset;

        [SerializeField]
        Vector2 cameraOffset;

        [SerializeField]
        Vector2 resetOffset;

        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask activatorMask;


        int hitCount;
        bool isInitHit;

        Vector2 initHitOffset;

        Collider2D[] hit;
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
            hit = new Collider2D[1];
            lastHitRef = null;
        }

        void Update()
        {
            if (isInitHit && (hitCount < 0) && lastHitRef) {

                if (isResetAfterExit) {
                    _ResetCameraOffset();
                }

                lastHitRef = null;
                isInitHit = false;
            }
        }

        void FixedUpdate()
        {
            hitCount = Physics2D.OverlapBoxNonAlloc(transform.position + new Vector3(offset.x, offset.y, 0.0f), size, 0.0f,  hit, activatorMask);

            if (hitCount > 0) {
                
                if (!isInitHit) {

                    var cameraFollow = Camera.main.GetComponent<CameraFolllow>();
                    initHitOffset = new Vector2(cameraFollow.OffsetX, cameraFollow.OffsetY);

                    _SetCameraOffset(cameraOffset);
                    isInitHit = true;
                }

                lastHitRef = hit[0];
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
            if (isResetByPreviousOffset) {
                _SetCameraOffset(initHitOffset);
            }
            else {
                _SetCameraOffset(resetOffset);
            }
        }
    }
}
