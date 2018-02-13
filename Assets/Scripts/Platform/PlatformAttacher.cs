using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class PlatformAttacher : MonoBehaviour
    {
        [SerializeField]
        bool isUse = true;

        [SerializeField]
        Vector2 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        float distance;

        [SerializeField]
        LayerMask platformMask;


        public bool IsUse { get { return isUse; } }


        bool isInitHit;

        Vector3 offsetFromPlatform;
        Vector3 contactPoint;
        Vector3 lastHitPosition;

        Rigidbody2D rigid;
        RaycastHit2D hit;


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(
                transform.position + new Vector3(offset.x, offset.y, 0.0f),
                new Vector3(size.x, size.y, 0.0f)
            ); 
        }
#endif

        void Awake()
        {
            rigid = GetComponent<Rigidbody2D>();
        }

        void FixedUpdate()
        {
            hit = Physics2D.BoxCast(transform.position + new Vector3(offset.x, offset.y, 0.0f), size, 0.0f, Vector2.down, distance, platformMask);

            if (hit) {

                if (!isInitHit && hit.distance <= 0.1f) {

                    Debug.Log("Hit Platform..");

                    contactPoint = transform.position;
                    lastHitPosition = hit.transform.position;

                    offsetFromPlatform = contactPoint - hit.transform.position;
                    isInitHit = true;
                }
            }
            else {
                isInitHit = false;
            }
        }

        void LateUpdate()
        {
            _AttachHandler();
        }

        void _AttachHandler()
        {
            if (!hit || !isUse) {
                return;
            }

            if (isInitHit) {

                if (lastHitPosition != hit.transform.position) {

                    var dir = (hit.transform.position - contactPoint);
                    dir.x += offsetFromPlatform.x;

                    rigid.MovePosition(contactPoint + dir);
                }
            }
        }

        public void Use(bool value)
        {
            isUse = value;

            if (!value) {
                isInitHit = false;
            }
        }
    }
}
