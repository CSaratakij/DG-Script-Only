using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SpritePlotter : EditorWindow
{
    const string SPRITE_PARENT = "Plotter_Sprite";
    const string TILED_PARENT = "Plotter_Tiled";


    public enum SpriteMode
    {
        Tiled,
        Simple
    }

    public enum EditMode
    {
        None,
        Multiple,
        Single
    }

    public enum GridPresetState
    {
        View,
        Edit
    }

    [SerializeField]
    Vector2 scrollPos;

    [SerializeField]
    Vector2 gridPresetScrollPos;

    [SerializeField]
    Vector2Int gridPresetSize = new Vector2Int(1, 1);

    [SerializeField]
    Vector2Int currentGridPresetSize = new Vector2Int(1, 1);

    [SerializeField]
    Sprite currentSprite;

    [SerializeField]
    bool isUse;

    [SerializeField]
    bool isUsePreset;

    [SerializeField]
    bool isUseSnap;

    [SerializeField]
    bool isBeginPlot;

    [SerializeField]
    int presetLength = 1;

    [SerializeField]
    float offsetX = 1.0f;

    [SerializeField]
    SortingLayer sortingLayer;

    [SerializeField]
    int sortingOrder = 0;

    [SerializeField]
    SpriteMode spriteMode;

    [SerializeField]
    EditMode currentMode;

    [SerializeField]
    int currentEditModeTab;

    [SerializeField]
    int currentGridPresetTab = 0;

    [SerializeField]
    string[] strAvailableEditMode = new string[] { "None", "Multiple", "Single" };

    [SerializeField]
    Sprite[] spritePresets = new Sprite[1];

    [SerializeField]
    bool isFoldSelectGridPreset;

    [SerializeField]
    int selectedGridPresetIndex;

    [SerializeField]
    int previousSelectedPresetIndex;

    [SerializeField]
    TextAsset fileSpriteGridPresetProfile;

    [SerializeField]
    string[] spriteGridPresetNames = new string[0];

    [SerializeField]
    Sprite[] spriteGridPresets = new Sprite[1];

    [SerializeField]
    SpriteGridProfile currentSpriteGridProfile;


    [Serializable]
    class SpriteGridProfile
    {
        public SpriteGridPreset[] presets = null;
    }

    [Serializable]
    class SpriteGridPreset
    {
        public string name = string.Empty;
        public Vector2Int size = new Vector2Int(1, 1);
        public string[] spriteAssetPath = null;
    }

    int pressCount;
    int currentSelectSortingLayerIndex;
    int selectedGridIndex;

    bool isFoldProfileSetting;

    Vector3 beginPos;
    Vector3 endPos;

    Editor gameObjectEditor;


    [MenuItem("Custom/Plotter/SpritePlotter")]
    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(SpritePlotter));
    }

    void OnEnable()
    {
        if (spriteGridPresets.Length <= 0) {
            spriteGridPresets = new Sprite[1];
            currentGridPresetSize = new Vector2Int(1, 1);
        }

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
            _CopySprite_Handler(e, mousePos);
        }
    }

    void OnGUI()
    {
        _GUIHandler();
    }

    void _GUIHandler()
    {
        GUILayout.Label ("Setting", EditorStyles.boldLabel);

        isUse = EditMode.None != currentMode;
        isBeginPlot = isUse;

        currentEditModeTab = GUILayout.Toolbar(currentEditModeTab, strAvailableEditMode);
        currentMode = (EditMode)currentEditModeTab;

        if (currentSprite) {
            gameObjectEditor = Editor.CreateEditor(currentSprite);
            gameObjectEditor.OnPreviewGUI(GUILayoutUtility.GetRect(100, 100), EditorStyles.whiteLabel);
        }

        currentSprite = (Sprite)EditorGUILayout.ObjectField(currentSprite, typeof(Sprite), true);

        if (EditMode.Multiple == currentMode) {
            spriteMode = (SpriteMode)EditorGUILayout.EnumPopup("Sprite Mode", spriteMode);
        }

        var sortingLayerNames = new string[SortingLayer.layers.Length];

        for (int i = 0; i < sortingLayerNames.Length; i++) {
            var id = SortingLayer.layers[i].id;
            sortingLayerNames[i] = SortingLayer.IDToName(id);
        }

        currentSelectSortingLayerIndex = EditorGUILayout.Popup("Sorting Layer", currentSelectSortingLayerIndex, sortingLayerNames);

        sortingLayer = SortingLayer.layers[currentSelectSortingLayerIndex];
        sortingOrder = EditorGUILayout.IntSlider("Sorting Order", sortingOrder, -10, 10);

        if (isBeginPlot) {
            isUseSnap = EditorGUILayout.Toggle("Use Snap", isUseSnap);
        }

        if (EditMode.Multiple == currentMode && SpriteMode.Simple == spriteMode) {

            offsetX = EditorGUILayout.FloatField("Offset X", offsetX);
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
        else {

            isFoldSelectGridPreset = EditorGUILayout.Foldout(isFoldSelectGridPreset, "Grid Preset");

            if (isFoldSelectGridPreset) {
                isFoldProfileSetting = EditorGUILayout.Foldout(isFoldProfileSetting, "Profile Setting");

                if (isFoldProfileSetting) {
                    fileSpriteGridPresetProfile = (TextAsset)EditorGUILayout.ObjectField(
                        new GUIContent(""),
                        fileSpriteGridPresetProfile,
                        typeof(TextAsset),
                        false
                    );

                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                    if (GUILayout.Button("Load")) {

                        if (!fileSpriteGridPresetProfile) {
                            EditorUtility.DisplayDialog("Warning", "Select profile's file first..", "OK");
                        }
                        else {
                            var isLoadGridPreset = EditorUtility.DisplayDialog(
                                "Load Grid Preset",
                                "Are you sure to load grid preset?\n" + "All unsaved preset will be lost.",
                                "Load Preset",
                                "Cancel"
                            );

                            if (isLoadGridPreset) {
                                _Load_From_SaveProfile();
                            }
                        }
                    }

                    if (GUILayout.Button("New") && fileSpriteGridPresetProfile) {

                        var isNewProfile = EditorUtility.DisplayDialog(
                            "Make a new Grid Preset",
                            "Are you sure to create a new grid profile?",
                            "Make a new grid profile",
                            "Cancel"
                        );

                        if (isNewProfile) {
                            isNewProfile = EditorUtility.DisplayDialog(
                                "Make a new Grid Preset : Confirm",
                                "Are you really sure about Making a new grid profile?",
                                "Ofcourse, Make a new one please..",
                                "Cancel"
                            );
                        }

                        //Need Refactor
                        if (isNewProfile && fileSpriteGridPresetProfile) {
                            var profile = new SpriteGridProfile();
                            var currentPreset = new SpriteGridPreset();

                            currentPreset.name = "Untitled";
                            currentPreset.size = new Vector2Int(1, 1);
                            currentPreset.spriteAssetPath = new string[1];

                            profile.presets = new SpriteGridPreset[1];
                            profile.presets[0] = currentPreset;

                            var json = JsonUtility.ToJson(profile, true);
                            var path = AssetDatabase.GetAssetPath(fileSpriteGridPresetProfile);

                            using (var writer = new StreamWriter(path)) {
                                writer.Write(json);
                            }

                            AssetDatabase.ImportAsset(path);
                            _Load_From_SaveProfile();
                        }
                    }

                    EditorGUILayout.EndHorizontal();
                }

                if (!fileSpriteGridPresetProfile) {
                    return;
                }

                selectedGridPresetIndex = EditorGUILayout.Popup(selectedGridPresetIndex, spriteGridPresetNames);
                currentGridPresetTab = GUILayout.Toolbar(currentGridPresetTab, new string[] { "View", "Edit" });

                if (selectedGridPresetIndex != previousSelectedPresetIndex) {
                    _Reload_Sprite_From_Cached(selectedGridPresetIndex);
                    previousSelectedPresetIndex = selectedGridPresetIndex;
                }

                if (currentGridPresetTab == 1) {

                    gridPresetSize = EditorGUILayout.Vector2IntField("", gridPresetSize);

                    gridPresetSize.x = Mathf.Clamp(gridPresetSize.x, 1, gridPresetSize.x);
                    gridPresetSize.y = Mathf.Clamp(gridPresetSize.y, 1, gridPresetSize.y);

                    if (GUILayout.Button("Resize")) {
                        if (gridPresetSize.x > 0 && gridPresetSize.y > 0) {

                            var isResizeGridPreset = EditorUtility.DisplayDialog(
                                "Resize Grid Preset",
                                "Are you sure to resize current grid preset?",
                                "Resize",
                                "Cancel"
                            );

                            if (isResizeGridPreset) {
                                currentGridPresetSize = gridPresetSize;

                                var oldChunk = spriteGridPresets;
                                spriteGridPresets = new Sprite[currentGridPresetSize.x * currentGridPresetSize.y];

                                if (oldChunk.Length > spriteGridPresets.Length) {
                                    for (int i = 0; i < spriteGridPresets.Length; i++) {
                                        spriteGridPresets[i] = oldChunk[i];
                                    }
                                }
                                else if (oldChunk.Length < spriteGridPresets.Length) {
                                    for (int i = 0; i < oldChunk.Length; i++) {
                                        spriteGridPresets[i] = oldChunk[i];
                                    }
                                }
                            }
                        }
                    }

                    gridPresetScrollPos = EditorGUILayout.BeginScrollView(gridPresetScrollPos);

                        if (spriteGridPresets.Length > 0) {
                            var nextFirstRowIndex = 0;

                            for (int j = 0; j < currentGridPresetSize.y; j++) {
                                EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                                for (int i = 0; i < currentGridPresetSize.x; i++) {
                                    spriteGridPresets[i + nextFirstRowIndex] = (Sprite)EditorGUILayout.ObjectField(
                                        new GUIContent(""),
                                        spriteGridPresets[i + nextFirstRowIndex],
                                        typeof(Sprite),
                                        false,
                                        GUILayout.MaxWidth(50),
                                        GUILayout.MaxHeight(50)
                                    );
                                }

                                EditorGUILayout.EndHorizontal();
                                nextFirstRowIndex += currentGridPresetSize.x;
                            }
                        }

                    EditorGUILayout.EndScrollView();

                    EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

                        if (GUILayout.Button("Save")) {

                            var isSavePreset = EditorUtility.DisplayDialog(
                                "Save Grid Preset",
                                "Are you sure to save current grid preset?",
                                "Save",
                                "Cancel"
                            );

                            //Need Refactor
                            if (isSavePreset && fileSpriteGridPresetProfile) {
                                var currentPreset = currentSpriteGridProfile.presets[selectedGridPresetIndex];

                                //Need UI that can change preset name..
                                //And avoid conflict name (same preset at one profile)
                                /* currentPreset.name = "A1"; */

                                currentPreset.size = currentGridPresetSize;
                                currentPreset.spriteAssetPath = new string[spriteGridPresets.Length];

                                for (int i = 0; i < spriteGridPresets.Length; i++) {
                                    currentPreset.spriteAssetPath[i] = AssetDatabase.GetAssetPath(spriteGridPresets[i]);
                                }

                                var json = JsonUtility.ToJson(currentSpriteGridProfile, true);
                                var path = AssetDatabase.GetAssetPath(fileSpriteGridPresetProfile);

                                using (var writer = new StreamWriter(path)) {
                                    writer.Write(json);
                                }

                                AssetDatabase.ImportAsset(path);
                            }
                        }

                        if (GUILayout.Button("Clear")) {

                            var isClearGridPreset = EditorUtility.DisplayDialog(
                                "Clear Grid Preset",
                                "Are you sure to clear current grid preset?",
                                "Clear",
                                "Cancel"
                            );

                            if (isClearGridPreset) {
                                for (int i = 0; i < spriteGridPresets.Length; i++) {
                                    spriteGridPresets[i] = null;
                                }
                            }
                        }

                    EditorGUILayout.EndHorizontal();
                }

                if (currentGridPresetTab == 0 && spriteGridPresets.Length > 0) {

                    var styles = new GUIStyle();

                    styles.margin = GUI.skin.button.margin;
                    styles.imagePosition = ImagePosition.ImageOnly;
                    styles.stretchWidth = false;
                    styles.stretchHeight = false;

                    gridPresetScrollPos = EditorGUILayout.BeginScrollView(gridPresetScrollPos);

                        var nextFirstRowIndex = 0;

                        for (int j = 0; j < currentGridPresetSize.y; j++) {
                            GUILayout.BeginHorizontal(GUILayout.MaxWidth(100));

                            for (int i = 0; i < currentGridPresetSize.x; i++) {
                                styles.normal.background = AssetPreview.GetAssetPreview(spriteGridPresets[i + nextFirstRowIndex]);
                                styles.active.background = styles.normal.background;

                                if (GUILayout.Button("", styles, GUILayout.Width(50), GUILayout.Height(50))) {
                                    selectedGridIndex = i + nextFirstRowIndex;
                                    currentSprite = spriteGridPresets[selectedGridIndex];
                                }
                            }

                            GUILayout.EndHorizontal();
                            nextFirstRowIndex += currentGridPresetSize.x;
                        }

                    EditorGUILayout.EndScrollView();
                }
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

            component.sortingLayerID = sortingLayer.id;
            component.sortingLayerName = sortingLayer.name;

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

                    component.sortingLayerID = sortingLayer.id;
                    component.sortingLayerName = sortingLayer.name;

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

                component.sortingLayerID = sortingLayer.id;
                component.sortingLayerName = sortingLayer.name;

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

    void _CopySprite_Handler(Event e, Vector2 mousePos)
    {
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

    void _Load_From_SaveProfile()
    {
        try {
            currentSpriteGridProfile = JsonUtility.FromJson<SpriteGridProfile>(fileSpriteGridPresetProfile.text);

            currentGridPresetSize = currentSpriteGridProfile.presets[0].size;
            gridPresetSize = currentGridPresetSize;

            var loadedPresetnames = new string[currentSpriteGridProfile.presets.Length];

            for (int i = 0; i < loadedPresetnames.Length; i++) {
                loadedPresetnames[i] = currentSpriteGridProfile.presets[i].name;
            }

            selectedGridPresetIndex = 0;
            previousSelectedPresetIndex = -1;

            spriteGridPresetNames = loadedPresetnames;

            //Need Refactor
            var loadedSprite = new Sprite[currentSpriteGridProfile.presets[0].spriteAssetPath.Length];

            for (int i = 0; i < currentSpriteGridProfile.presets[0].spriteAssetPath.Length; i++) {
                var path = currentSpriteGridProfile.presets[0].spriteAssetPath[i];
                loadedSprite[i] = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            }

            spriteGridPresets = loadedSprite;
        }
        catch (Exception exception) {
            EditorUtility.DisplayDialog(
                "Error",
                "Can't load current grid profile from file : " + fileSpriteGridPresetProfile.name,
                "Sad"
            );
            Debug.LogException(exception);
        }
    }

    void _Reload_Sprite_From_Cached(int index)
    {
        try {
            var loadedSprite = new Sprite[currentSpriteGridProfile.presets[index].spriteAssetPath.Length];

            for (int i = 0; i < currentSpriteGridProfile.presets[index].spriteAssetPath.Length; i++) {
                var path = currentSpriteGridProfile.presets[index].spriteAssetPath[i];
                loadedSprite[i] = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
            }

            currentGridPresetSize = currentSpriteGridProfile.presets[index].size;
            gridPresetSize = currentGridPresetSize;

            spriteGridPresets = loadedSprite;
        }
        catch (Exception exception) {
            EditorUtility.DisplayDialog("Error", "Can't load sprite from current profile's cache at preset index : " + index, "Sad");
            Debug.LogException(exception);
        }
    }
}
