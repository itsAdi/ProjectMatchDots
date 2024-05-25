using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace KemothStudios.Utility.ScriptableObjects
{
    public sealed class ScriptableObjectFromClass : EditorWindow
    {
        private bool _initialized;
        public string FilePath;

        [MenuItem("Assets/Kemoth Studios/ScriptableObjects/Create ScriptableObject")]
        public static void CreateScriptableObject()
        {
            ScriptableObjectFromClass window = ScriptableObject.CreateInstance<ScriptableObjectFromClass>();
            window.FilePath = Path.GetDirectoryName(AssetDatabase.GetAssetPath(Selection.activeObject));
            window.position = new Rect(0f, 0f, 300f, 65f);
            window.titleContent = new GUIContent("Create ScriptableObject");
            window.minSize = new Vector2(300f, 65f);
            window.ShowModalUtility();
        }

        private void OnGUI()
        {
            if (!_initialized)
            {
                Vector2 mousePos = GUIUtility.GUIToScreenPoint(Event.current.mousePosition);
                position = new Rect(mousePos.x, mousePos.y, position.width, position.height);
                _initialized = true;
            }
        }

        void CreateGUI()
        {
            var text = new TextField("Name");
            rootVisualElement.Add(text);

            var CreateButton = new Button();
            CreateButton.text = "Create";
            CreateButton.clicked += () =>
            {
                if (!string.IsNullOrEmpty(text.value) && !string.IsNullOrWhiteSpace(text.value))
                {
                    UnityEngine.Object asset = ScriptableObject.CreateInstance(((MonoScript)Selection.activeObject).GetClass().Name);
                    string name = AssetDatabase.GenerateUniqueAssetPath($"{FilePath}/{text.text}.asset");
                    AssetDatabase.CreateAsset(asset, name);
                    AssetDatabase.SaveAssets();

                    EditorUtility.FocusProjectWindow();

                    Selection.activeObject = asset;
                    Close();
                }
            };
            rootVisualElement.Add(CreateButton);

            var cancelButton = new Button();
            cancelButton.text = "Cancel";
            cancelButton.clicked += () =>
            {
                Close();
            };
            rootVisualElement.Add(cancelButton);
        }

        [MenuItem("Assets/Kemoth Studios/ScriptableObjects/Create ScriptableObject", true)]
        public static bool IsValidClass()
        {
            Type type = Selection.activeObject.GetType();
            if (type == typeof(MonoScript))
            {
                type = ((MonoScript)Selection.activeObject).GetClass().BaseType;
                bool stop = false;
                bool gotValidType = false;
                while (!stop)
                {
                    if (type == null)
                    {
                        stop = true;
                    }
                    else if (type == typeof(ScriptableObject))
                    {
                        stop = true;
                        gotValidType = true;
                    }
                    else
                    {
                        type = type.BaseType;
                    }
                }
                return gotValidType;
            }
            return false;
        }
    }
}