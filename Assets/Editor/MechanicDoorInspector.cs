using UnityEngine;
using UnityEditor;
using DG;

[CustomEditor(typeof(MechanicDoor))]
[CanEditMultipleObjects]
public class MechanicDoorInspector : Editor
{
    SerializedProperty offset;
    SerializedProperty size;
    SerializedProperty targetDoor;
    SerializedProperty targetSceneIndex;
    SerializedProperty wrapID;
    SerializedProperty allowEnterMask;
    SerializedProperty useInstantOpen;
    SerializedProperty targetType;


    bool showAreaSetting;
    MechanicDoor door;


    void OnEnable()
    {
        door = (MechanicDoor)target;

        offset = serializedObject.FindProperty("offset");
        size = serializedObject.FindProperty("size");
        targetDoor = serializedObject.FindProperty("targetDoor");
        targetSceneIndex = serializedObject.FindProperty("targetSceneIndex");
        wrapID = serializedObject.FindProperty("wrapID");
        allowEnterMask = serializedObject.FindProperty("allowEnterMask");
        useInstantOpen = serializedObject.FindProperty("useInstantOpen");
        targetType = serializedObject.FindProperty("targetType");
    }

    public override void OnInspectorGUI()
    {
        _Draw_Target_Setting();
        EditorGUILayout.Space();

        _Draw_Door_Setting();
        serializedObject.ApplyModifiedProperties();
    }

    void _Draw_Door_Setting()
    {
        showAreaSetting = EditorGUILayout.Foldout(showAreaSetting, "Area");
        if (showAreaSetting) {
            EditorGUILayout.PropertyField(offset, new GUIContent("Offset"));
            EditorGUILayout.PropertyField(size, new GUIContent("Size"));
        }

        EditorGUILayout.PropertyField(useInstantOpen, new GUIContent("Instant Open"));
        EditorGUILayout.PropertyField(allowEnterMask, new GUIContent("Allow Enter"));
    }

    void _Draw_Target_Setting()
    {
        GUILayout.Label ("Target", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(targetType, new GUIContent("Type")); 

        if (door.IsUseTargetDoor) {
            _Draw_TargetDoor_Setting();
        }
        else if (door.IsUseTargetScene) {
            _Draw_Scene_Setting();
        }
    }

    void _Draw_TargetDoor_Setting()
    {
        EditorGUILayout.PropertyField(targetDoor, new GUIContent("Door"));
    }

    void _Draw_Scene_Setting()
    {
        EditorGUILayout.PropertyField(targetSceneIndex, new GUIContent("Build Index"));
        EditorGUILayout.PropertyField(wrapID, new GUIContent("Wrap ID"));
    }
}
