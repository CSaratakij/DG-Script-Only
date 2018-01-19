using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DG.Editor
{
    public class GameScene : EditorWindow
    {
        bool isShowAllScene = true;
        Vector2 scrollPos;

        List<SceneAsset> scenes = new List<SceneAsset>();

        SceneAsset customMainScene;
        SceneAsset targetEditorScene;

        static string editModeScene;


        [MenuItem("Window/CustomPlayButton")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GameScene));
        }

        void OnEnable()
        {
            EditorApplication.playModeStateChanged += _OnPlayModeStateChanged;
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
            titleContent.text = "GameScene";
            EditorGUI.BeginDisabledGroup(EditorApplication.isPlayingOrWillChangePlaymode);
                _PlayHandler();
                EditorGUILayout.Space();
                _ShowAllSceneAsset();
            EditorGUI.EndDisabledGroup();
        }

        static void _OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state == PlayModeStateChange.EnteredEditMode) {
                EditorSceneManager.playModeStartScene = (SceneAsset)AssetDatabase.LoadAssetAtPath(editModeScene, typeof(SceneAsset));
            }
        }

        void _PlayHandler()
        {
            GUILayout.Label ("Play", EditorStyles.boldLabel);
            customMainScene = (SceneAsset)EditorGUILayout.ObjectField(
                new GUIContent("Scene:"),
                customMainScene,
                typeof(SceneAsset),
                false);

            if (GUILayout.Button("Play"))
            {
                if (!EditorApplication.isPlayingOrWillChangePlaymode) {

                    if (customMainScene != null) {
                        editModeScene = EditorSceneManager.GetActiveScene().path;
                        EditorSceneManager.playModeStartScene = customMainScene;
                        EditorApplication.isPlaying = true;
                    }
                    else {
                        EditorUtility.DisplayDialog("Error", "No scene selected for playing.", "OK");
                    }
                }
            }
        }

        void _ShowAllSceneAsset()
        {
            GUILayout.Label("Editor", EditorStyles.boldLabel);
            isShowAllScene = EditorGUILayout.Foldout(isShowAllScene, "Scenes");

            if (isShowAllScene) {

                if (GUILayout.Button("Refresh")) {

                    scenes.Clear();
                    var assetsGUID = AssetDatabase.FindAssets("t:SceneAsset");

                    foreach (var id in assetsGUID) {

                        var path = AssetDatabase.GUIDToAssetPath(id);
                        var asset = (SceneAsset)AssetDatabase.LoadAssetAtPath(path, typeof(SceneAsset));

                        scenes.Add(asset);
                    }
                }

                EditorGUILayout.Space();

                scrollPos = EditorGUILayout.BeginScrollView(scrollPos, false, false);
                    for (int i = 0; i < scenes.Count; i++) {
                        scenes[i] = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent(""),
                                scenes[i],
                                typeof(SceneAsset),
                                false);
                    }

                EditorGUILayout.EndScrollView();
            }
        }
    }
}
