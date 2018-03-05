using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class Coin : Item
    {
        [SerializeField]
        uint point = 1;


        public delegate void FuncValueChanged(uint value);
        public static FuncValueChanged OnPointValueChanged;

        public static uint TotalPoint;
        public uint Point { get { return point; } }


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var label = string.Format("Point : {0}", Point);
            Handles.color = Color.blue;
            Handles.Label(transform.position, label);
        }
#endif

        public override void Collect()
        {
            base.Collect();
            Coin.Add(1);
        }

        public static void FireEvent_PointValueChanged(uint value)
        {
            if (OnPointValueChanged != null) {
                OnPointValueChanged(value);
            }
        }

        public static void Add(uint value)
        {
            Coin.TotalPoint += value;
            FireEvent_PointValueChanged(Coin.TotalPoint);
        }

        public static void Remove(uint value)
        {
            Coin.TotalPoint -= value;
            FireEvent_PointValueChanged(Coin.TotalPoint);
        }
    }
}
