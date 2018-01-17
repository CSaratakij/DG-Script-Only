using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    [RequireComponent(typeof(Animator))]
    [RequireComponent(typeof(Switch))]
    public class PlateSwitch : MonoBehaviour
    {
        [SerializeField]
        Vector3 offset;

        [SerializeField]
        Vector2 size;

        [SerializeField]
        LayerMask layerMask;


        Animator anim;
        Switch switchObj;

        Collider2D hit;


        void Awake()
        {
            anim = GetComponent<Animator>();
            switchObj = GetComponent<Switch>();
        }

        void Update()
        {
            if (hit)
            {
                _Open();
            }
            else {
                _Close();
            }
        }

        void FixedUpdate()
        {
            hit = Physics2D.OverlapBox(transform.position + offset, size, 0.0f, layerMask);
        }

#if UNITY_EDITOR
        void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, size);
            Handles.Label(transform.position + offset, "Trigger Area");
        }
#endif

        void _Open()
        {
            anim.Play("Open");
            switchObj.TurnOn();
        }

        void _Close()
        {
            anim.Play("Close");
            switchObj.TurnOff();
        }
    }
}
