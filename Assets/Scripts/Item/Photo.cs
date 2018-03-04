using System.Linq;
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

        [SerializeField]
        uint partID;


        public delegate void FuncCollect(uint id, uint partID);
        public static event FuncCollect OnPhotoCollected;
        public static Dictionary<uint, List<uint>> Unlocked_Photo_List = new Dictionary<uint, List<uint>>();

        public uint ID { get { return photoID; } } 
        public uint PartID { get { return partID; } } 


#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var lblID = string.Format("ID : {0}", photoID);
            var lblPartID = string.Format("Part ID : {0}", partID);
            Handles.color = Color.blue;
            Handles.Label(transform.position, lblID);
            Handles.Label(transform.position - (Vector3.up * 0.2f), lblPartID);
        }
#endif

        public override void Collect()
        {
            base.Collect();
            _Unlock_Photo_By_ID();
            _FireEvent_OnPhotoCollected();
        }

        void _Unlock_Photo_By_ID()
        {
            var isPhotoExist = Unlocked_Photo_List.ContainsKey(ID);

            if (isPhotoExist) {
                var isPartExist = Unlocked_Photo_List[ID].Contains(PartID);

                if (!isPartExist) {
                    Unlocked_Photo_List[ID].Add(PartID);
                }
            }
            else {
                Unlocked_Photo_List.Add(ID, new List<uint>());
                Unlocked_Photo_List[ID].Add(PartID);
            }
        }

        void _FireEvent_OnPhotoCollected()
        {
            if (OnPhotoCollected != null) {
                OnPhotoCollected(photoID, partID);
            }
        }
    }
}
