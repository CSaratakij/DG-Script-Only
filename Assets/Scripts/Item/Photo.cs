using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
    using UnityEditor;
#endif

namespace DG
{
    public class Photo : Item
    {
        [SerializeField]
        uint photoID;


        public uint ID { get { return photoID; } } 


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var label = string.Format("ID : {0}", photoID);
            Handles.color = Color.blue;
            Handles.Label(transform.position, label);
        }
#endif

        public override void Collect()
        {
            base.Collect();
            //Todo
            //Send Unlock info on runtime.
        }
    }
}
