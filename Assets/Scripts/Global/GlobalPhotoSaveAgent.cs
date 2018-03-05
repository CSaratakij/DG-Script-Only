using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class GlobalPhotoSaveAgent : SaveInstance
    {
        string json = "";

        struct GlobalPhotoInfo
        {
            public Dictionary<uint, List<uint>> photoInfo;
        }

        void _SerializeToJson()
        {
            var info = new GlobalPhotoInfo();
            info.photoInfo = Photo.Unlocked_Photo_List;
            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<GlobalPhotoInfo>(json);
            Photo.Unlocked_Photo_List = info.photoInfo;
        }

        public override void Save()
        {
            _SerializeToJson();
            SaveInstance.InsertRecord(masterKey, key, id, json);
        }

        public override void Load()
        {
            var temp = SaveInstance.LoadRecord(masterKey, key, id);

            if (temp != "") {
                _DeserializeToInfo(temp);
            }
        }
    }
}
