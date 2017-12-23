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
        float offsetX;

        [SerializeField]
        [Range(-5.0f, 5.0f)]
        float offsetY;

        [SerializeField]
        [Range(-5.0f, 5.0f)]
        float boundYMargin;
        
        [SerializeField]
        float dampSpeedX;
        
        [SerializeField]
        float dampSpeedY;

        [SerializeField]
        float dampSpeedOutMarginY;


        public bool IsStickYAxis { get { return isStickY; } }
        public bool IsOutBoundY { get { return expectOutBoundY; } }


        Vector3 offset;
        Vector3 bottomLeftWorldPoint;

        bool isNeedFollowX;
        bool isNeedFollowY;

        bool expectOutBoundY;

        bool isStickY;
        bool isInitStickY;

        float currentVerticalDistance;


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
            if (isEnableFollowing)
            {
                offset = (target.position - transform.position);

                if (Mathf.Abs(offset.x) > marginX) {
                    isNeedFollowX = true;
                }
                else if (Mathf.Abs(offset.x) <= 0.1f) {
                    isNeedFollowX = false;
                }

                bottomLeftWorldPoint = Camera.main.ScreenToWorldPoint(Vector3.zero);
                expectOutBoundY = (target.position.y - boundYMargin) < bottomLeftWorldPoint.y;

                if (expectOutBoundY) {

                    if (!isInitStickY && currentVerticalDistance == 0.0f) {
                        currentVerticalDistance = transform.position.y - (target.position.y + offsetY);
                        isInitStickY = true;
                    }

                    isStickY = true;
                }

                if (isStickY) {
                    _StickYAxis();
                    isNeedFollowY = false;
                }
                else {
                    isNeedFollowY = true;
                }

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

                var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, dampSpeedX);
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

                var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, dampSpeedY);

                newPos.x = transform.position.x;
                newPos.z = POSITION_Z;

                transform.position = newPos;
            }
        }

        void _StickYAxis()
        {
            if (target) {
                var newPos = transform.position;

                var targetPos = target.position;
                targetPos.y = target.position.y + offsetY;

                if (transform.position.y > targetPos.y) {
                    currentVerticalDistance -= Mathf.Lerp(currentVerticalDistance, 0.0f, dampSpeedOutMarginY) * Time.deltaTime;
                    newPos.y = targetPos.y + currentVerticalDistance;
                }
                else {
                    newPos.y = targetPos.y;
                }

                newPos.x = transform.position.x;
                newPos.z = POSITION_Z;

                transform.position = newPos;
            }
        }

        void _FollowTarget()
        {
            if (target) {
                var currentVelocity = Vector3.zero;
                var targetPos = transform.position + offset;
                targetPos.x += offsetX;
                targetPos.y += offsetY;

                var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, 0.08f);
                newPos.z = POSITION_Z;
                transform.position = newPos;
            }
            else {
                Debug.Log("Can't find target..");
            }
        }

        public void LockCamera()
        {
            isEnableFollowing = false;
        }

        public void UnlockCamera()
        {
            isEnableFollowing = true;
        }

        public void ToggleFollow()
        {
            isEnableFollowing = !isEnableFollowing;
        }

        public void UnStickYAxis() {
            isStickY = false;
            isInitStickY = false;
            currentVerticalDistance = 0.0f;
        }
    }
}
