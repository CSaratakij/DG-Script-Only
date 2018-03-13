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
        Vector2 groundOffset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        float distance;

        [SerializeField]
        float platformSpeed;

        [SerializeField]
        float moveSpeed;

        [SerializeField]
        LayerMask platformMask;


        public bool IsUse { get { return isUse; } }
        public Vector3 ExtraStep { get { return extraStep; } set { extraStep = value; } }


        bool isInitHit;

        Vector3 offsetFromPlatform;
        Vector3 contactPoint;
        Vector3 extraStep;

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
            hit = Physics2D.Raycast(transform.position + new Vector3(offset.x, offset.y, 0.0f), Vector2.down, distance, platformMask);

            if (hit) {

                if (!isInitHit && hit.distance <= 0.1f) {

                    Debug.Log("Hit Platform..");
                    contactPoint = transform.position;

                    offsetFromPlatform = hit.transform.position - contactPoint;
                    offsetFromPlatform.y += groundOffset.y;

                    isInitHit = true;
                }

                if (!isUse) {

                    var platformControl = hit.transform.gameObject.GetComponent<PlatformController>();

                    if (!platformControl.IsPauseMoving) {

                        var axisX = Input.GetAxisRaw("Horizontal");

                        if (platformControl.MoveDirection.x > 0.0f) {

                            if (axisX > 0.0f ) {
                                var velocity = rigid.velocity;

                                var extraSpeed = Vector2.right * moveSpeed;
                                extraSpeed *= Time.deltaTime;

                                velocity.x = rigid.velocity.x + extraSpeed.x;
                                rigid.velocity = velocity;
                            }
                        }
                        else if (platformControl.MoveDirection.x < 0.0f) {

                            if (axisX < 0.0f ) {
                                var velocity = rigid.velocity;

                                var extraSpeed = Vector2.left * moveSpeed;
                                extraSpeed *= Time.deltaTime;

                                velocity.x = rigid.velocity.x + extraSpeed.x;
                                rigid.velocity = velocity;
                            }
                        }
                    }
                }
            }
            else {
                isInitHit = false;
            }

            _AttachHandler();
        }

        void _AttachHandler()
        {
            if (!hit || !isUse) {
                return;
            }

            if (isInitHit) {
                MoveAlongPlatform();
            }
        }

        public void Use(bool value)
        {
            isUse = value;

            if (!value) {
                isInitHit = false;
            }
        }

        public void MoveAlongPlatform()
        {
            float weightX = Mathf.Cos(Time.deltaTime * platformSpeed * 2 * Mathf.PI) * 0.5f + 0.5f;
            float weightY = Mathf.Sin(Time.deltaTime * platformSpeed * 2 * Mathf.PI) * 0.5f + 0.5f;

            var newPos = transform.position;

            newPos.x = transform.position.x * weightX;
            newPos.y = transform.position.y * weightY;

            var resultPos = newPos;

            resultPos.x = newPos.x + (hit.transform.position.x - offsetFromPlatform.x) * (1 - weightX);
            resultPos.y = newPos.y + (hit.point.y - offsetFromPlatform.y) * (1 - weightY);

            rigid.MovePosition(resultPos);
        }

        public Vector3 GetExpectPos()
        {
            float weight = Mathf.Cos(Time.deltaTime * platformSpeed * 2 * Mathf.PI) * 0.5f + 0.5f;
            return transform.position * weight + (hit.transform.position - offsetFromPlatform) * (1 - weight);
        }
    }
}
