using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

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

        void OnDrawGizmosSelected() {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(transform.position + offset, size);
            Handles.Label(transform.position + offset, "Trigger Area");
        }

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
