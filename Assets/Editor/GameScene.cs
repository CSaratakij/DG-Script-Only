using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace DG.Editor
{
    public class GameScene : EditorWindow
    {
        SceneAsset targetEditorScene;


        [MenuItem("Window/CustomPlayButton")]
        public static void ShowWindow()
        {
            EditorWindow.GetWindow(typeof(GameScene));
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

        void _PlayHandler()
        {
            GUILayout.Label ("Play", EditorStyles.boldLabel);
            EditorSceneManager.playModeStartScene = (SceneAsset)EditorGUILayout.ObjectField(new GUIContent("Scene:"),
                    EditorSceneManager.playModeStartScene,
                    typeof(SceneAsset),
                    false);

            if (GUILayout.Button("Play"))
            {
                if (!EditorApplication.isPlaying && !EditorApplication.isCompiling) {
                    if (EditorSceneManager.playModeStartScene != null) {
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

            if (GUILayout.Button("Load in Editor")) {
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
