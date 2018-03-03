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


        public static List<uint> Unlocked_Photo_List = new List<uint>();
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
            _Unlock_Photo_By_ID();
        }

        void _Unlock_Photo_By_ID()
        {
            var isAlreadyUnlocked = Unlocked_Photo_List.Contains(ID);
            if (!isAlreadyUnlocked) {
                Unlocked_Photo_List.Add(ID);
            }
        }
    }
}
