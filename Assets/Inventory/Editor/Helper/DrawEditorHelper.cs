using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Helper
{
    public static class DrawEditorHelper
    {
        public static void DrawDefaultHeader(string headerText)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(headerText, EditorStyles.boldLabel);
            EditorGUILayout.EndHorizontal();
        }

        public static bool CreateToggleProperty(bool currentInputValue)
        {
            return GUILayout.Toggle(currentInputValue, new GUIContent("to remove from editor..."), new GUIStyle()
            {
                font = new Font()
            });
        }

        public static T CreateObjectField<T>(string title, T objectField) where T : Object
        {
            return (T) EditorGUILayout.ObjectField(new GUIContent(title), objectField, typeof(T), true);
        }
    }
}