using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CollisionPlotter : EditorWindow
{
    const string COLLIDER_PARENT = "Plotter_Collider";
    const string COLLISION_PARENT = "Plotter_Collision";


    public enum ColliderType
    {
        None,
        BoxCollider,
        EdgeCollider
    }

    [SerializeField]
    bool isUse;

    [SerializeField]
    bool showAllCollision;

    [SerializeField]
    bool isUseSnap;

    [SerializeField]
    string colliderTag = "Untagged";

    [SerializeField]
    int colliderLayer;

    [SerializeField]
    ColliderType currentType;

    [SerializeField]
    int currentColliderTab;

    [SerializeField]
    string[] strAvailableCollider = new string[] { "None", "Box", "Edge" };


    public static int pressCount = 0;

    public static bool isTrigger;
    public static bool isBeginPlot;


    Vector3 beginPos;
    Vector3 endPos;


    [MenuItem("Custom/Plotter/CollisionPlotter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(CollisionPlotter));
    }

    void OnEnable()
    {
        Tools.current = Tool.None;
        SceneView.onSceneGUIDelegate += OnSceneGUI;
    }

    void OnDestroy()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    void OnSceneGUI(SceneView sceneView)
    {
        if (isUse) {
            _Scene_Input_Handler();
            _Plot_Collider_Handler();
            SceneView.lastActiveSceneView.Repaint();
        }
    }

    void _Scene_Input_Handler()
    {
        var e = Event.current;

        switch (e.type) {
            case EventType.KeyDown:
                if (e.keyCode == KeyCode.C) {
                    pressCount = 0;
                    isBeginPlot = !isBeginPlot;
                    Repaint();
                }
                else if (e.keyCode == KeyCode.S) {
                    isUseSnap = !isUseSnap;
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
            Handles.Label(mousePos + Vector2.up * 0.6f, (isBeginPlot) ? "[ Using Ploter ]" : "");

            Handles.color = Color.yellow;
            Handles.Label(mousePos + Vector2.up * 0.5f, (isUseSnap) ? "Snap : On" : "Snap : Off");

            Handles.color = Color.red;
            Handles.DrawLine(beginPos, mousePos);
        }

        if (e.control) {
            pressCount = 0;
            Tools.current = Tool.None;

            Handles.color = Color.white;
            Handles.Label(mousePos + Vector2.up * 0.6f, "[ Delete Mode ]");

            Handles.color = Color.yellow;
            Handles.DrawWireCube(mousePos, new Vector2(0.5f, 0.5f));

            if (e.type == EventType.MouseDown && e.button == 0) {
                _DeleteColliderHandler(mousePos);

                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                GUIUtility.hotControl = controlId;

                Event.current.Use();
            }
        }
    }

    void OnGUI()
    {
        _GUIHandler();
    }

    void _GUIHandler()
    {
        GUILayout.Label ("Setting", EditorStyles.boldLabel);

        currentColliderTab = GUILayout.Toolbar(currentColliderTab, strAvailableCollider);
        currentType = (ColliderType)currentColliderTab;

        isUse = ColliderType.None != currentType;
        isBeginPlot = isUse;

        colliderTag = EditorGUILayout.TagField("Tag", colliderTag);
        colliderLayer = EditorGUILayout.LayerField("Layer", colliderLayer);

        _Always_Show_Collider_Handler();
        isTrigger = EditorGUILayout.Toggle ("Use Trigger", isTrigger);

        if (isBeginPlot) {
            isUseSnap = EditorGUILayout.Toggle("Use Snap", isUseSnap);
        }
    }

    void _Always_Show_Collider_Handler()
    {
        showAllCollision = EditorGUILayout.Toggle ("Always Show Collider", showAllCollision);
        Physics2D.alwaysShowColliders = showAllCollision;
    }

    void _Plot_Collider_Handler()
    {
        if (isBeginPlot) {
            switch (currentType) {
                case ColliderType.BoxCollider:
                    _Plot_Box_Collider();
                break;

                case ColliderType.EdgeCollider:
                    _Plot_Edge_Collider();
                break;
            }
        }
    }

    void _Plot_Box_Collider()
    {
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

                    if (isUseSnap) {
                        beginPos = _Snap(1, beginPos);
                    }
                }
                else if (pressCount == 2) {
                    var sceneViewCamera = SceneView.lastActiveSceneView.camera; 
                    var mousePos = e.mousePosition;

                    mousePos.y = sceneViewCamera.pixelHeight - mousePos.y;
                    endPos = sceneViewCamera.ScreenToWorldPoint(mousePos);

                    if (isUseSnap) {
                        endPos = _Snap(1, endPos);
                    }

                    _CreateBoxCollider2D(isTrigger, beginPos, endPos);
                    SceneView.RepaintAll();

                    pressCount = 0;
                }
                break;
        }
    }

    void _Plot_Edge_Collider()
    {
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

                    if (isUseSnap) {
                        beginPos = _Snap(1, beginPos);
                    }
                }
                else if (pressCount == 2) {
                    var sceneViewCamera = SceneView.lastActiveSceneView.camera; 
                    var mousePos = e.mousePosition;

                    mousePos.y = sceneViewCamera.pixelHeight - mousePos.y;
                    endPos = sceneViewCamera.ScreenToWorldPoint(mousePos);

                    if (isUseSnap) {
                        endPos = _Snap(1, endPos);
                    }

                    _CreateEdgeCollider2D(isTrigger, beginPos, endPos);
                    SceneView.RepaintAll();

                    pressCount = 0;
                }
                break;
        }
    }

    void _CreateBoxCollider2D(bool isTrigger, Vector3 beginPos, Vector3 endPos)
    {
        var objName = (isTrigger) ? (LayerMask.LayerToName(colliderLayer) + "_ground_collider") : (LayerMask.LayerToName(colliderLayer) + "_ground_collision");

        var obj = new GameObject(objName);
        var component = obj.AddComponent(typeof(BoxCollider2D)) as BoxCollider2D;

        var relativePos = (endPos - beginPos);
        var halfRelativePos = relativePos / 2.0f;

        var expectPos = beginPos + halfRelativePos;
        expectPos.z = 0.0f;

        var expectSize = halfRelativePos;
        obj.transform.position = expectPos;

        obj.tag = colliderTag;
        obj.layer = colliderLayer;

        component.isTrigger = isTrigger;
        component.size = new Vector2(Mathf.Abs(expectSize.x * 2.0f), Mathf.Abs(expectSize.y * 2.0f));

        var parentObjName = (isTrigger) ? COLLIDER_PARENT : COLLISION_PARENT;
        var parent = GameObject.Find(parentObjName);

        if (!parent) {
            parent = new GameObject(parentObjName);
            parent.transform.position = Vector3.zero;

            Undo.RegisterCreatedObjectUndo(parent, "Created parent of a new box collider 2D..");
        }

        obj.transform.SetParent(parent.transform);
        Undo.RegisterCreatedObjectUndo(obj, "Created a new box collider 2D..");
    }

    void _CreateEdgeCollider2D(bool isTrigger, Vector3 beginPos, Vector3 endPos)
    {
        var objName = (isTrigger) ? (LayerMask.LayerToName(colliderLayer) + "_edge_collider") : (LayerMask.LayerToName(colliderLayer) + "_edge_collision");

        var obj = new GameObject(objName);
        var component = obj.AddComponent(typeof(EdgeCollider2D)) as EdgeCollider2D;

        var relativePos = (endPos - beginPos);
        var halfRelativePos = relativePos / 2.0f;

        var expectPos = beginPos + halfRelativePos;
        expectPos.z = 0.0f;

        obj.tag = colliderTag;
        obj.layer = colliderLayer;

        component.isTrigger = isTrigger;
        component.points = new Vector2[] {
            beginPos,
            endPos
        };

        var parentObjName = (isTrigger) ? COLLIDER_PARENT : COLLISION_PARENT;
        var parent = GameObject.Find(parentObjName);

        if (!parent) {
            parent = new GameObject(parentObjName);
            parent.transform.position = Vector3.zero;

            Undo.RegisterCreatedObjectUndo(parent, "Created parent of a new edge collider 2D..");
        }

        obj.transform.SetParent(parent.transform);
        Undo.RegisterCreatedObjectUndo(obj, "Created a new edge collider 2D..");
    }

    Vector3 _Snap(float value, Vector3 target)
    {
        var depth = 0;
        var snapInverse = 1 / value;

        var result = Vector3.zero;

        result.x = Mathf.Round(target.x * value) / snapInverse;
        result.y = Mathf.Round(target.y * value) / snapInverse;
        result.z = depth;

        return result;
    }

    void _DeleteColliderHandler(Vector3 origin)
    {
        var results = new Collider2D[1];
        Physics2D.OverlapBoxNonAlloc(origin, new Vector2(0.5f, 0.5f), 0.0f, results);

        if (results.Length <= 0) {
            return;
        }

        if (results[0] != null) {
            Undo.DestroyObjectImmediate(results[0].transform.gameObject);
        }
    }
}
