using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CollisionPloter : EditorWindow
{
    [SerializeField]
    bool showAllCollision;


    public static int pressCount = 0;

    public static bool isTrigger;
    public static bool isBeginPlot;


    Vector3 beginPos;
    Vector3 endPos;


    [MenuItem("Window/ColisionPloter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CollisionPloter));
    }

    void OnEnable()
    {
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        _Scene_Input_Handler();
        _Plot_Collider_Handler();

        SceneView.lastActiveSceneView.Repaint();
    }

    void _Scene_Input_Handler()
    {
        var e = Event.current;

        switch (e.type) {
            case EventType.KeyDown:
                if (e.keyCode == KeyCode.F) {
                    pressCount = 0;
                    isBeginPlot = !isBeginPlot;
                    Repaint();
                }

                if (isBeginPlot) {
                    if (e.keyCode == KeyCode.T) {
                        isTrigger = !isTrigger;
                        Repaint();
                    }
                }
                break;
        }

        var mousePos = e.mousePosition;
        var sceneViewCamera = SceneView.lastActiveSceneView.camera;

        mousePos.y = sceneViewCamera.pixelHeight - mousePos.y;
        mousePos = sceneViewCamera.ScreenToWorldPoint(mousePos);

        if (pressCount == 1) {

            Handles.color = Color.white;
            Handles.Label(mousePos + Vector2.up * 0.5f, (isBeginPlot) ? "Using Ploter" : "");

            Handles.color = Color.red;
            Handles.DrawLine(beginPos, mousePos);
        }
    }

    void OnGUI()
    {
        _GUIHandler();
    }

    void _GUIHandler()
    {
        GUILayout.Label ("Setting", EditorStyles.boldLabel);
        _Always_Show_Collider_Handler();

        isBeginPlot = EditorGUILayout.Toggle ("Start Ploting", isBeginPlot);
        isTrigger = EditorGUILayout.Toggle ("Is Trigger", isTrigger);
    }

    void _Always_Show_Collider_Handler()
    {
        showAllCollision = EditorGUILayout.Toggle ("Always Show Collider", showAllCollision);
        Physics2D.alwaysShowColliders = showAllCollision;
    }

    void _Plot_Collider_Handler()
    {
        if (!isBeginPlot) {
            return;
        }

        var e = Event.current;

        switch (e.type) {

            case EventType.MouseDown:

                if (e.button != 0) {
                    if (e.button == 1) {
                        pressCount = 0;
                    }
                    return;
                }

                pressCount += 1;

                if (pressCount == 1) {

                    var sceneViewCamera = SceneView.lastActiveSceneView.camera; 
                    var mousePos = e.mousePosition;

                    mousePos.y = sceneViewCamera.pixelHeight - mousePos.y;
                    beginPos = sceneViewCamera.ScreenToWorldPoint(mousePos);
                }
                else if (pressCount == 2) {
                    var sceneViewCamera = SceneView.lastActiveSceneView.camera; 
                    var mousePos = e.mousePosition;

                    mousePos.y = sceneViewCamera.pixelHeight - mousePos.y;
                    endPos = sceneViewCamera.ScreenToWorldPoint(mousePos);

                    _CreateBoxCollider2D(isTrigger, beginPos, endPos);
                    SceneView.RepaintAll();

                    pressCount = 0;
                }
                break;
        }
    }

    void _CreateBoxCollider2D(bool isTrigger, Vector3 beginPos, Vector3 endPos)
    {
        var objName = (isTrigger) ? "ground_collider" : "ground_collision";

        var obj = new GameObject(objName);
        var component = obj.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

        var relativePos = (endPos - beginPos);
        var halfRelativePos = relativePos / 2.0f;

        var expectPos = beginPos + halfRelativePos;
        expectPos.z = 0.0f;

        var expectSize = halfRelativePos;
        obj.transform.position = expectPos;

        component.isTrigger = isTrigger;
        component.size = new Vector2(Mathf.Abs(expectSize.x * 2.0f), Mathf.Abs(expectSize.y * 2.0f));

        Undo.RegisterCreatedObjectUndo(obj, "Created new box collider 2D..");
    }
}
