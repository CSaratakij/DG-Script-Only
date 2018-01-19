using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DG.Editor
{
    public class GameScene : EditorWindow
    {
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
            _PlayHandler();
            _EditorSceneHandler();
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
                }
            }
        }

        void _EditorSceneHandler()
        {
            GUILayout.Label ("Editor", EditorStyles.boldLabel);
            targetEditorScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Scene:"),
                    targetEditorScene,
                    typeof(SceneAsset),
                    false);

            if (GUILayout.Button("Open in Editor")) {
                if (!EditorApplication.isPlaying && !EditorApplication.isCompiling) {
                    if (targetEditorScene != null)
                    {
                        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
                            var path = AssetDatabase.GetAssetPath(targetEditorScene);
                            EditorSceneManager.OpenScene(path);
                        }
                    }
                }
            }
        }
    }
}
