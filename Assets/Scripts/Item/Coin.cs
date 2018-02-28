using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DG
{
    public class Coin : Item
    {
        [SerializeField]
        uint point = 1;


        public static uint TotalPoint;
        public uint Point { get { return point; } }


        public override void Collect()
        {
            base.Collect();
            TotalPoint += 1;
        }
    }
}
