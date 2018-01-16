using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class FocusSaveAgent : SaveInstance
    {
        string json = "";
        WorldWrappingController worldWrappingControl;


        struct FocusSaveInfo
        {
            public bool unlocked_MoveMode;
            public bool unlocked_EditMode;
        }


        void _SerializeToJson()
        {
            var info = new FocusSaveInfo();
            worldWrappingControl = GetComponent<WorldWrappingController>();

            if (worldWrappingControl) {
                info.unlocked_MoveMode = worldWrappingControl.IsCanMoveMode;
                info.unlocked_EditMode = worldWrappingControl.IsCanEditMode;
            }

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<FocusSaveInfo>(json);
            worldWrappingControl = GetComponent<WorldWrappingController>();

            if (worldWrappingControl) {
                worldWrappingControl.IsCanMoveMode = info.unlocked_MoveMode;
                worldWrappingControl.IsCanEditMode = info.unlocked_EditMode;
            }
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
