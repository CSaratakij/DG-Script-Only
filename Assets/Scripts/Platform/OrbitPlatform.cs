using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class OrbitPlatform : MonoBehaviour
    {
        [SerializeField]
        float rotationSpeed;

        [SerializeField]
        OrbitDirection direction;


        enum OrbitDirection
        {
            Left,
            Right
        }

        void Update()
        {
            _MakePlatformOrbit();
        }

        void _MakePlatformOrbit()
        {
            foreach (Transform child in transform) {
                var previousRotation = child.rotation;
                var rotateDirection = (OrbitDirection.Left == direction) ? -1.0f : 1.0f;

                child.RotateAround(transform.position, Vector3.forward, (rotateDirection * rotationSpeed) * Time.deltaTime);
                child.rotation = previousRotation;
            }
        }
    }
}
