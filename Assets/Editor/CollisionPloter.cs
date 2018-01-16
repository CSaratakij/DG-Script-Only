using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using DG;

public class CollisionPloter : EditorWindow
{
    bool showAllCollision;


    [MenuItem("Window/ColisionPloter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CollisionPloter));
    }

    void OnGUI()
    {
        _GUIHandler();
    }

    void _GUIHandler()
    {
        GUILayout.Label ("Setting", EditorStyles.boldLabel);
        showAllCollision = EditorGUILayout.Toggle ("Show Collision", showAllCollision);
        _ToggleShowCollision();
    }

    void _ToggleShowCollision()
    {
        if (showAllCollision) {
        }
        else {
        }
    }
}
