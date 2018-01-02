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
        float slowDampSpeedX;
        
        [SerializeField]
        float dampSpeedY;

        [SerializeField]
        float dampSpeedOutMarginY;


        public bool IsStickYAxis { get { return isStickY; } }
        public bool IsOutBoundY { get { return expectOutBoundY; } }
        public float OffsetX { get { return offsetX; } }


        Vector3 offset;
        Vector3 bottomLeftWorldPoint;
        Vector3 lastPositionBeforeLock;
        
        bool isNeedFollowX;
        bool isNeedFollowY;

        bool isSlowFollowX;
        bool expectOutBoundY;

        bool isStickY;
        bool isInitStickY;

        bool isLock;

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
                if (isLock) {
                    _Follow(lastPositionBeforeLock, dampSpeedX);
                }
                else {
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
                            currentVerticalDistance = 0.0f;
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

                        if (isSlowFollowX) {
                            _FollowHorizontal(slowDampSpeedX);
                        }
                        else {
                            _FollowHorizontal(dampSpeedX);
                        }
                    }

                    if (isNeedFollowY) {
                        _FollowVertical();
                    }
                }
            }
        }

        void _Follow(Vector3 targetPos, float dampSpeed) {
            targetPos.y += offsetY;

            var currentVelocity = Vector3.zero;
            var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, dampSpeed);

            newPos.z = POSITION_Z;
            transform.position = newPos;
        }

        void _FollowHorizontal(float dampSpeed)
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

                var bottomDistanceFromPlayer = ((bottomLeftWorldPoint.y + boundYMargin) - target.position.y);
                var bottomPosRelativeToCamera = (transform.position.y - bottomDistanceFromPlayer);
                var possibleTargetYAxisMax = (target.position.y + offsetY);

                if (target.position.y < possibleTargetYAxisMax) {

                    currentVerticalDistance += Mathf.Lerp(0.0f, (transform.position.y - possibleTargetYAxisMax), dampSpeedOutMarginY) * Time.deltaTime;
                    newPos.y = bottomPosRelativeToCamera - currentVerticalDistance;
                }

                newPos.x = transform.position.x;
                newPos.z = POSITION_Z;

                transform.position = newPos;
            }
        }

        public void LockCamera()
        {
            lastPositionBeforeLock = target.position;
            isLock = true;
        }

        public void UnlockCamera()
        {
            isLock = false;
        }

        public void ToggleFollow()
        {
            isEnableFollowing = !isEnableFollowing;
        }

        public void SetFollowX(bool value)
        {
            isNeedFollowX = value;
        }

        public void SetFollowY(bool value)
        {
            isNeedFollowX = value;
        }

        public void SetOffsetX(float value)
        {
            offsetX = value;
        }

        public void SlowFollowX(bool value)
        {
            isSlowFollowX = value;
        }

        public void FlipOffsetX()
        {
            offsetX *= -1.0f;
        }

        public void UnStickYAxis() {
            isStickY = false;
            isInitStickY = false;
            currentVerticalDistance = 0.0f;
        }
    }
}
