﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpritePloter : EditorWindow
{
    const string SPRITE_PARENT = "Plotter_Sprite";
    const string TILED_PARENT = "Plotter_Tiled";

    public static bool isBeginPlot;

    public enum SpriteMode
    {
        Simple,
        Tiled
    }

    public enum EditMode
    {
        Single,
        Multiple
    }

    [SerializeField]
    Vector2 scrollPos;

    [SerializeField]
    Sprite currentSprite;

    [SerializeField]
    bool isUse;

    [SerializeField]
    bool isUsePreset;

    [SerializeField]
    bool isUseSnap;

    [SerializeField]
    bool isOverrideSprite; //replace sprite on the same position..

    [SerializeField]
    int presetLength = 1;

    [SerializeField]
    float offsetX = 1.0f;

    [SerializeField]
    int sortingOrder = 0;

    [SerializeField]
    SpriteMode spriteMode;

    [SerializeField]
    EditMode currentMode;

    [SerializeField]
    Sprite[] spritePresets = new Sprite[1];


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

        if (e.control) {
            pressCount = 0;
            Tools.current = Tool.None;

            Handles.color = Color.white;
            Handles.Label(mousePos + Vector2.up * 0.6f, "[ Delete Mode ]");

            Handles.color = Color.red;
            Handles.DrawWireCube(mousePos, new Vector2(0.5f, 0.5f));

            if (e.type == EventType.MouseDown && e.button == 0) {

                var pickedGameObject = HandleUtility.PickGameObject(e.mousePosition, false);

                if (pickedGameObject) {
                    Undo.DestroyObjectImmediate(pickedGameObject);
                }

                var controlId = GUIUtility.GetControlID(FocusType.Passive);
                GUIUtility.hotControl = controlId;

                e.Use();
            }
        }

        if (e.shift) {

            Handles.color = Color.white;
            Handles.Label(mousePos + Vector2.up * 0.6f, "[ Copy Mode ]");

            Handles.color = Color.magenta;
            Handles.DrawWireCube(mousePos, new Vector2(0.6f, 0.6f));

            if (e.type == EventType.MouseDown && e.button == 0) {

                var pickedGameObject = HandleUtility.PickGameObject(e.mousePosition, false);

                if (pickedGameObject) {
                    var renderer = pickedGameObject.GetComponent<SpriteRenderer>();

                    if (!renderer) {
                        var message = string.Format("Can't copy sprite on object : '{0}'", pickedGameObject.name);
                        SceneView.lastActiveSceneView.ShowNotification(new GUIContent(message));
                    }

                    currentSprite = (renderer) ? renderer.sprite : currentSprite;
                    Repaint();

                    var controlId = GUIUtility.GetControlID(FocusType.Passive);
                    GUIUtility.hotControl = controlId;

                    e.Use();
                }
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

        isUse = EditorGUILayout.Toggle("Use", isUse);

        offsetX = EditorGUILayout.FloatField("Offset X", offsetX);
        sortingOrder = EditorGUILayout.IntField("Sorting Order", sortingOrder);

        currentSprite = (Sprite)EditorGUILayout.ObjectField(currentSprite, typeof(Sprite), true);

        currentMode = (EditMode)EditorGUILayout.EnumPopup("Edit Mode", currentMode);
        spriteMode = (SpriteMode)EditorGUILayout.EnumPopup("Sprite Mode", spriteMode);

        isBeginPlot = EditorGUILayout.Toggle("Begin Plot", isBeginPlot);

        if (isBeginPlot) {
            isOverrideSprite = EditorGUILayout.Toggle("Override Sprite", isOverrideSprite);
            isUseSnap = EditorGUILayout.Toggle("Use Snap", isUseSnap);
        }

        if (EditMode.Multiple == currentMode && SpriteMode.Simple == spriteMode) {
            isUsePreset = EditorGUILayout.Toggle("Use Preset", isUsePreset);

            if (isUsePreset) {

                presetLength = EditorGUILayout.IntField("Preset Length", presetLength);

                if (GUILayout.Button("Create new Preset")) {
                    spritePresets = new Sprite[presetLength];
                }

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);

                    for (int i = 0; i < spritePresets.Length; i++) {
                        spritePresets[i] = (Sprite)EditorGUILayout.ObjectField(
                            new GUIContent("Sprite"),
                            spritePresets[i],
                            typeof(Sprite),
                            false);
                    }

                EditorGUILayout.EndScrollView();
            }
        }
    }

    void _Plot_Sprite_Handler()
    {
        if (isBeginPlot) {
            Tools.current = Tool.None;

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
            component.sortingOrder = sortingOrder;

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


        if (e.button == 0 && e.isMouse) {
            pressCount += 1;
        }
        else if (e.button == 1 && e.isMouse) {
            pressCount = 0;
        }
        else {
            return;
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

        var offset = new Vector3(offsetX, 1.0f, 0.0f);
        var currentSpriteIndex = 0;

        switch (spriteMode) {
            case SpriteMode.Simple:

            for (int i = 0; i < total_horizontal; i++) {

                for (int j = 0; j < total_vertical; j++) {

                    var objName = "Sprite_" + i.ToString();
                    var obj = new GameObject(objName);

                    var component = obj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;

                    if (isUsePreset) {
                        var spriteFromPresets = spritePresets[currentSpriteIndex];
                        currentSpriteIndex = ((currentSpriteIndex + 1) > (presetLength - 1)) ? 0 : currentSpriteIndex + 1;

                        component.sprite = spriteFromPresets;
                    }
                    else {
                        component.sprite = currentSprite;
                    }

                    component.sortingOrder = sortingOrder;

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

                        Undo.RegisterCreatedObjectUndo(parent, "Created parent of a new sprites.");
                    }

                    obj.transform.SetParent(parent.transform);
                    Undo.RegisterCreatedObjectUndo(obj, "Created new sprites..");
                }
            }
                break;

            case SpriteMode.Tiled:
            {
                var objName = string.Format("Tield_{0}", (currentSprite) ? currentSprite.name : "Unknown");
                var obj = new GameObject(objName);

                var component = obj.AddComponent(typeof(SpriteRenderer)) as SpriteRenderer;

                var center = new Vector3(
                        Mathf.Abs(relativePos.x),
                        Mathf.Abs(relativePos.y),
                        0.0f);

                center *= 0.5f;

                var expectPos = beginPos;

                if (endPos.x > beginPos.x) {
                    expectPos.x = beginPos.x + center.x;
                }
                else if (endPos.x < beginPos.x) {
                    expectPos.x = beginPos.x - center.x;
                }

                if (beginPos.y < endPos.y) {
                    expectPos.y = beginPos.y + center.y;
                }
                else if (beginPos.y > endPos.y) {
                    expectPos.y = beginPos.y - center.y;
                }

                obj.transform.position = expectPos;

                component.drawMode = SpriteDrawMode.Tiled;
                component.sprite = currentSprite;
                component.size = new Vector2(total_horizontal, total_vertical);
                component.sortingOrder = sortingOrder;
                
                var parent = GameObject.Find(TILED_PARENT);

                if (!parent) {
                    parent = new GameObject(TILED_PARENT);
                    parent.transform.position = Vector3.zero;

                    Undo.RegisterCreatedObjectUndo(parent, "Created parent of a new tiled sprite");
                }

                obj.transform.SetParent(parent.transform);
                Undo.RegisterCreatedObjectUndo(obj, "Create a new sprite with draw mode 'Tiled'");
            }
                break;
        }
    }

    void _CopySprite()
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