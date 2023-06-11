using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Utils
{
    public static class GUIRushUtility
    {
        public static Rect NextLine(this Rect position)
        {
            position.y += EditorGUIUtility.singleLineHeight;

            return position;
        }
    }
}