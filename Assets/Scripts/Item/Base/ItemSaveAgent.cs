using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;

namespace DG
{
    public class ItemSaveAgent : SaveInstance
    {
        Item item;
        string json = "";

        struct ItemSaveInfo
        {
            public bool isUsed;
        }

        protected override void Awake()
        {
            base.Awake();
            item = GetComponent<Item>();
        }

        void _SerializeToJson()
        {
            var info = new ItemSaveInfo();
            info.isUsed = item.IsUsed;

            json = JsonConvert.SerializeObject(info, Formatting.None);
        }

        void _DeserializeToInfo(string json)
        {
            var info = JsonConvert.DeserializeObject<ItemSaveInfo>(json);
            item.IsUsed = info.isUsed;
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

        public override void FinishLoad()
        {
            if (item.IsUsed) {
                gameObject.SetActive(false);
            }
        }
    }
}
