using UnityEngine;

namespace DG
{
    public class CameraFolllow : MonoBehaviour
    {
        const float POSITION_Z = -10.0f;

        [SerializeField]
        bool isEnableFollowing;

        [SerializeField]
        Transform target;

        [SerializeField]
        [Range(-5.0f, 5.0f)]
        float marginX;

        [SerializeField]
        [Range(-5.0f, 5.0f)]
        float marginY;

        [SerializeField]
        [Range(-5.0f, 5.0f)]
        float offsetX;

        [SerializeField]
        [Range(-5.0f, 5.0f)]
        float offsetY;

        [SerializeField]
        float dampSpeed;


        Vector3 offset;


        bool isNeedFollowX;
        bool isNeedFollowY;


        void Start()
        {
            if (target) {
                var startPos = target.position;
                startPos.x += offsetX;
                startPos.y += offsetY;
                startPos.z = POSITION_Z;
                transform.position = startPos;
            }
        }

        void LateUpdate()
        {
            offset = (target.position - transform.position);

            if (Mathf.Abs(offset.x) > marginX) {
                isNeedFollowX = true;
            }
            else if (Mathf.Abs(offset.x) <= 0.38f) {
                isNeedFollowX = false;
            }

            if (Mathf.Abs(offset.y) > marginY) {
                isNeedFollowY = true;
            }
            else if (Mathf.Abs(offset.y) <= 1.0f) {
                isNeedFollowY = false;
            }

            if (isEnableFollowing)
            {
                if (isNeedFollowX) {
                    _FollowHorizontal();
                }

                if (isNeedFollowY) {
                    _FollowVertical();
                }
            }
        }

        void _FollowHorizontal()
        {
            if (target) {
                var currentVelocity = Vector3.zero;
                var targetPos = transform.position + offset;
                targetPos.x += offsetX;

                var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, dampSpeed);
                newPos.y = transform.position.y;
                newPos.z = POSITION_Z;
                transform.position = newPos;
            }
        }

        void _FollowVertical()
        {
            if (target) {
                var currentVelocity = Vector3.zero;
                var targetPos = transform.position + offset;
                targetPos.y += offsetY;

                var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, dampSpeed);
                newPos.x = transform.position.x;
                newPos.z = POSITION_Z;
                transform.position = newPos;
            }
        }

        void _FollowTarget()
        {
            if (target)
            {
                var currentVelocity = Vector3.zero;
                var targetPos = transform.position + offset;
                targetPos.x += offsetX;
                targetPos.y += offsetY;

                var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, 0.08f);
                newPos.z = POSITION_Z;
                transform.position = newPos;
            }
            else
            {
                Debug.Log("Can't find target..");
            }
        }

        public void ToggleFollow()
        {
            isEnableFollowing = !isEnableFollowing;
        }
    }
}
