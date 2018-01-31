using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpritePloter : EditorWindow
{
    const string SPRITE_PARENT = "Plotter_Sprite";

    public static bool isBeginPlot;

    public enum EditMode
    {
        Single,
        Multiple
    }

    [SerializeField]
    Sprite currentSprite;

    [SerializeField]
    bool isUse;

    [SerializeField]
    bool isUseSnap;

    [SerializeField]
    bool isOverrideSprite; //replace sprite on the same position..

    [SerializeField]
    EditMode currentMode;


    int pressCount = 0;

    Vector3 beginPos;
    Vector3 endPos;


    [MenuItem("Window/SpritePloter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpritePloter));
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
            _Plot_Sprite_Handler();
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
            break;
        }

        var mousePos = e.mousePosition;
        var sceneCamera = SceneView.lastActiveSceneView.camera;

        mousePos.y = sceneCamera.pixelHeight - mousePos.y;
        mousePos = sceneCamera.ScreenToWorldPoint(mousePos);

        if (pressCount == 1) {
            Handles.color = Color.white;
            Handles.Label(mousePos + Vector2.up * 0.6f, (isBeginPlot) ? "[ Using Ploter ]" : "");

            Handles.color = Color.yellow;
            Handles.Label(mousePos + Vector2.up * 0.5f, (isUseSnap) ? "Snap : On" : "Snap : Off");

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
        isUse = EditorGUILayout.Toggle("Use", isUse);

        currentSprite = (Sprite)EditorGUILayout.ObjectField(currentSprite, typeof(Sprite), true);
        isBeginPlot = EditorGUILayout.Toggle("Begin Plot", isBeginPlot);

        if (isBeginPlot) {
            isOverrideSprite = EditorGUILayout.Toggle("Override Sprite", isOverrideSprite);
            isUseSnap = EditorGUILayout.Toggle("Use Snap", isUseSnap);
            currentMode = (EditMode)EditorGUILayout.EnumPopup("Edit Mode", currentMode);
        }
    }

    void _Plot_Sprite_Handler()
    {
        if (isBeginPlot) {
            switch (Event.current.type) {
                case EventType.MouseDown:
                    if (EditMode.Single == currentMode) {
                        _Plot_Sprite_Single_Mode();
                    }
                    else if (EditMode.Multiple == currentMode) {
                        _Plot_Sprite_Multiple_Mode();
                    }
                break;
            }
        }

        if (EditMode.Single == currentMode) {
            pressCount = 0;
        }
    }

    void _Plot_Sprite_Single_Mode()
    {
        var e = Event.current;

        var mousePos = e.mousePosition;
        var sceneCamera = SceneView.lastActiveSceneView.camera;

        mousePos.y = sceneCamera.pixelHeight - mousePos.y;
        mousePos = sceneCamera.ScreenToWorldPoint(mousePos);

        if (e.button == 0) {

            var objName = "Sprite";
            var obj = new GameObject(objName);

            var offset = new Vector3(0.5f, 0.5f, 0.0f);
            var expectPos = mousePos;

            if (isUseSnap) {
                var clampPos = _Snap(1, mousePos);
                
                if (clampPos.x < mousePos.x) {
                    expectPos.x = clampPos.x + offset.x;
                }
                else if (clampPos.x > mousePos.x) {
                    expectPos.x = clampPos.x - offset.x;
                }

                if (clampPos.y > mousePos.y) {
                    expectPos.y = clampPos.y - offset.y;
                }
                else if (clampPos.y < mousePos.y) {
                    expectPos.y = clampPos.y + offset.y;
                }
            }

            obj.transform.position = expectPos;

            var component = obj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            component.sprite = currentSprite;

            var parent = GameObject.Find(SPRITE_PARENT);

            if (!parent) {
                parent = new GameObject(SPRITE_PARENT);
                parent.transform.position = Vector3.zero;

                Undo.RegisterCreatedObjectUndo(parent, "Created parent of a new sprite..");
            }

            obj.transform.SetParent(parent.transform);
            Undo.RegisterCreatedObjectUndo(obj, "Create a new sprite.");
        }
    }

    void _Plot_Sprite_Multiple_Mode()
    {
        var e = Event.current;

        var mousePos = e.mousePosition;
        var offset = new Vector3(0.5f, 0.5f, 0.0f);

        var sceneCamera = SceneView.lastActiveSceneView.camera;

        mousePos.y = sceneCamera.pixelHeight - mousePos.y;
        mousePos = sceneCamera.ScreenToWorldPoint(mousePos);


        if (e.button == 0) {
            pressCount += 1;
        }
        else if (e.button == 1) {
            pressCount = 0;
        }

        if (pressCount == 1) {
            beginPos = mousePos;

            if (isUseSnap) {

                var expectPos = mousePos;
                var clampPos = _Snap(1, mousePos);
                
                if (clampPos.x < mousePos.x) {
                    expectPos.x = clampPos.x + offset.x;
                }
                else if (clampPos.x > mousePos.x) {
                    expectPos.x = clampPos.x - offset.x;
                }

                if (clampPos.y > mousePos.y) {
                    expectPos.y = clampPos.y - offset.y;
                }
                else if (clampPos.y < mousePos.y) {
                    expectPos.y = clampPos.y + offset.y;
                }

                beginPos = expectPos;
            }
        }
        else if (pressCount == 2) {
            endPos = mousePos;

            if (isUseSnap) {

                var expectPos = mousePos;
                var clampPos = _Snap(1, mousePos);
                
                if (clampPos.x < mousePos.x) {
                    expectPos.x = clampPos.x + offset.x;
                }
                else if (clampPos.x > mousePos.x) {
                    expectPos.x = clampPos.x - offset.x;
                }

                if (clampPos.y > mousePos.y) {
                    expectPos.y = clampPos.y - offset.y;
                }
                else if (clampPos.y < mousePos.y) {
                    expectPos.y = clampPos.y + offset.y;
                }

                endPos = expectPos;
            }

            _CreateMultipleSprite(beginPos, endPos);
            pressCount = 0;
        }
    }

    void _CreateMultipleSprite(Vector3 beginPos, Vector3 endPos)
    {
        var relativePos = endPos - beginPos;

        var total_horizontal = Mathf.RoundToInt(relativePos.x);
        total_horizontal = Mathf.Abs(total_horizontal) + 1;

        var total_vertical = Mathf.RoundToInt(relativePos.y);
        total_vertical = Mathf.Abs(total_vertical) + 1;

        var offset = new Vector3(1.0f, 1.0f, 0.0f);

        for (int i = 0; i < total_horizontal; i++) {

            for (int j = 0; j < total_vertical; j++) {

                var objName = "Sprite_" + i.ToString();
                var obj = new GameObject(objName);

                var component = obj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;
                component.sprite = currentSprite;

                var expectPos = beginPos;

                if (endPos.x > beginPos.x) {
                    expectPos.x = beginPos.x + (offset.x * i);
                }
                else if (endPos.x < beginPos.x) {
                    expectPos.x = endPos.x + (offset.x * i);
                }


                if (beginPos.y > endPos.y) {
                    expectPos.y = beginPos.y - (offset.y * j);
                }
                else if (beginPos.y < endPos.y) {
                    expectPos.y = endPos.y - (offset.y * j);
                }

                obj.transform.position = expectPos;
                var parent = GameObject.Find(SPRITE_PARENT);

                if (!parent) {
                    parent = new GameObject(SPRITE_PARENT);
                    parent.transform.position = Vector3.zero;

                    Undo.RegisterCreatedObjectUndo(parent, "Created parent of a new box collider 2D..");
                }

                obj.transform.SetParent(parent.transform);
                Undo.RegisterCreatedObjectUndo(obj, "Created new sprites..");
            }
        }
    }

    void _CopySprite()
    {

    }

    void _PasteSprite()
    {

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
}
