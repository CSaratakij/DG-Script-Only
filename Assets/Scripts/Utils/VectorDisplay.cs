using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class VectorDisplay : MonoBehaviour
    {
        [SerializeField]
        Transform pointA;

        [SerializeField]
        Transform pointB;


#if UNITY_EDITOR
         void OnDrawGizmos() {
            if (!pointA || !pointB) {
                return;
            }

            Handles.color = Color.red;
            Handles.DrawLine(pointA.position, pointB.position);

            var relativeVector = pointB.position - pointA.position;
            Handles.Label(transform.position, "Normal : " + relativeVector.normalized);
         }
#endif
    }
}
