using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DG.Editor
{
    public class SaveSetting : EditorWindow
    {
        int initID = 0;
        bool toggleSetID;
        string keys;

        SaveInstance[] allObj;

        List<string> objKeyList = new List<string>();
        Dictionary<string, List<SaveInstance>> objDict = new Dictionary<string, List<SaveInstance>>();


        [MenuItem("Window/SaveSetting")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(SaveSetting));
        }
        
        void OnGUI()
        {
            _GUIHandler();
        }

        void OnInspectorUpdate()
        {
            Repaint();
        }

        void _GUIHandler()
        {
            titleContent.text = "SaveSetting";

            GUILayout.Label ("ID Setting", EditorStyles.boldLabel);
            EditorGUILayout.Space();

            toggleSetID = EditorGUILayout.Toggle("Edit:", toggleSetID);

            if (toggleSetID) {
                initID = EditorGUILayout.IntField("Start ID: ", initID);
            }
            else {
                initID = 0;
            }

            if (GUILayout.Button("Generate ID")) {
                _UpdateAllSaveAbleObject();
                _GenerateIDPerKey();
                _DeleteSaveGame(true);
                _ReportGenerateResult();
            }

            if (GUILayout.Button("Check Duplicate ID")) {
                _UpdateAllSaveAbleObject();
                _CheckDuplicateIDPerKey();
            }

            if (SaveInstance.IsSaveFileExists()) {
                if (GUILayout.Button("Delete SaveFile")) {
                    _DeleteSaveGame();
                }
            }
        }

        void _UpdateAllSaveAbleObject()
        {
            allObj = _GetAllSaveInstanceArray();

            objKeyList.Clear();
            objDict.Clear();

            foreach (SaveInstance instance in allObj) {
                _GatherKey(instance);
                _MapKeyToDict(instance);
            }
        }

        SaveInstance[] _GetAllSaveInstanceArray()
        {
            return Object.FindObjectsOfType(typeof(SaveInstance)) as SaveInstance[];
        }

        void _GatherKey(SaveInstance obj)
        {
            if (!objKeyList.Contains(obj.Key)) {
                objKeyList.Add(obj.Key);
            }
        }

        void _MapKeyToDict(SaveInstance obj)
        {
            if (!objDict.ContainsKey(obj.Key)) {
                objDict.Add(obj.Key, new List<SaveInstance>());
            }

            objDict[obj.Key].Add(obj);
        }

        void _GenerateIDPerKey()
        {
            foreach (var pair in objDict) {
                foreach (SaveInstance instance in objDict[pair.Key]) {
                    _GenerateID(pair.Key, initID);
                }
            }
        }

        void _GenerateID(string key, int initID)
        {
            var currentID = (uint)initID;
            foreach (SaveInstance instance in objDict[key]) {
                Undo.RecordObject(instance, "Editing save instance's id..");
                instance.ID = currentID;

                currentID += 1;
            }
        }

        void _ReportGenerateResult()
        {
            var message = string.Format("Re-Generate all Save ID in this scene..\n Total Key : {0}\n Total Save Instance : {1}\n", objKeyList.Count, allObj.Length);
            EditorUtility.DisplayDialog("Generate ID : Success", message, "OK");
        }

        void _DeleteSaveGame(bool forceDelete = false)
        {
            if (!forceDelete) {
                var isConfirm = EditorUtility.DisplayDialog("Delete SaveGame", "Are you sure to delete a save file?", "Delete", "Cancel");
                if (isConfirm) {
                    SaveInstance.DeleteSave();
                    EditorUtility.DisplayDialog("Generate ID : Success", "SaveGame has been deleted..", "OK");
                }
            }
            else {
                SaveInstance.DeleteSave();
            }
        }

        void _CheckDuplicateIDPerKey()
        {
            var dulplicateKeys = new List<string>();
            var dulplicateID = new List<uint>();

            foreach (var pair in objDict) {

                for (int i = 0; i < objDict[pair.Key].Count; i++) {

                    var testID = objDict[pair.Key][i].ID;
                    var total = 0;

                    for (int j = 0; j < objDict[pair.Key].Count; j++) {

                        if (objDict[pair.Key][j].ID == testID) {
                            total += 1;
                        }

                        if (total > 1) {

                            var key = pair.Key;
                            var id = objDict[key][j].ID;

                            if (!dulplicateKeys.Contains(key)) {
                                dulplicateKeys.Add(key);
                            }

                            dulplicateID.Add(id);

                            break;
                        }
                    }
                }
            }

            if (dulplicateKeys.Count > 0) {

                var logConflictKey = string.Format("\tKey : {0}", dulplicateKeys.Count);
                var logConflictID = string.Format("\tSave Instance : {0}", dulplicateID.Count);

                EditorUtility.DisplayDialog("Verify ID : Error",
                            "[ Duplicate ID ] :\n" +
                            "Please generate new ID...\n" + 
                            "\n" +
                            "Total conflicts : \n" +
                            logConflictKey +
                            "\n" +
                            logConflictID,
                        "OK");
            }
            else {
                EditorUtility.DisplayDialog("Verify ID : Success",
                        "No conflicts Key and ID.\n" +
                        "No need to generate new ID.",
                        "OK");
            }
        }
    }
}
