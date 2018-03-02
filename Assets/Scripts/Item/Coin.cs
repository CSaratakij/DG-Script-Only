using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


        public override void Collect()
        {
            base.Collect();
            TotalPoint += 1;
            _FireEvent_PointValueChanged(TotalPoint);
        }

        void _FireEvent_PointValueChanged(uint value)
        {
            if (OnPointValueChanged != null) {
                OnPointValueChanged(value);
            }
        }
    }
}
