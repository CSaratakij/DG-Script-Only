using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class FocusEffector : MonoBehaviour
    {
        [SerializeField]
        bool isActive;

        [SerializeField]
        GameObject target;

        [SerializeField]
        Vector2 boundfreeOffset;

        [SerializeField]
        LayerMask boundMask;

        [SerializeField]
        Transform upperBound;
        
        [SerializeField]
        Transform lowerBound;

        [SerializeField]
        Transform rightBound;

        [SerializeField]
        Transform leftBound;


        public bool IsActive { get { return isActive; } }


        WorldWrappingController worldWrappingControl;


        void Awake()
        {
            _Initialilze();
        }

        void Update()
        {
            if (isActive) {
                _WrappingHandler();
            }
        }

        void _Initialilze()
        {
            //instantiate bound from prefabs here..
            //for dev purpose -> normal setup would be fine..
        }

        void _WrappingHandler()
        {

        }

        public void UseEffector(bool value)
        {
            isActive = value;
        }
    }
}
