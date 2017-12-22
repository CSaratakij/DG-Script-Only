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
        float dampSpeedX;
        
        [SerializeField]
        float dampSpeedY;

        [SerializeField]
        float dampSpeedOutMarginY;


        Vector3 offset;


        bool isNeedFollowX;
        bool isNeedFollowY;
        bool isForceFollowY;


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

                if (isForceFollowY) {
                    if (transform.position.y < (target.position.y + offsetY)) {
                        _FollowVertical();
                    }
                }
                else {
                    if (Mathf.Abs(offset.y) > marginY) {
                        isNeedFollowY = true;

                        //test.. out bound
                        //var expectOutBound = (offset.y < marginY - 0.5f);

                        /*
                        var expectOutBound = (offset.y < 0.05f);

                        if (expectOutBound) {
                            _FollowVerticalFaster();
                        }
                        //
                        //*/
                    }
                    else if (Mathf.Abs(offset.y) <= 1.0f) {
                        isNeedFollowY = false;
                    }
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

        //test
        public void _FollowVerticalFaster()
        {
            if (target) {
                var currentVelocity = Vector3.zero;
                var targetPos = transform.position + offset;
                targetPos.y += offsetY;

                var newPos = Vector3.SmoothDamp(transform.position, targetPos, ref currentVelocity, dampSpeedOutMarginY);
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

        public void ForceFollowVertical()
        {
            isForceFollowY = true;
        }

        public void UnForceFollowVertical()
        {
            isForceFollowY = false;
        }
    }
}
