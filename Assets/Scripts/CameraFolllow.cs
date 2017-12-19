using UnityEngine;

namespace DG
{
    public class CameraFolllow : MonoBehaviour
    {
        [SerializeField]
        bool isEnableFollowing;

        [SerializeField]
        Transform target;


        void Update()
        {
            if (isEnableFollowing)
            {
                _FollowTarget();
            }
        }

        void _FollowTarget()
        {
            if (target)
            {
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
