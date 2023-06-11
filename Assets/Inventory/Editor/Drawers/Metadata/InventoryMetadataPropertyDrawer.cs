using Inventory.Editor.Utils;
using Inventory.Scripts.Inventory.ItemsMetadata;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Drawers.Metadata
{
    [CustomPropertyDrawer(typeof(InventoryMetadata))]
    public class InventoryMetadataPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // TODO: Implement this metadata
            EditorGUI.BeginProperty(position, label, property);

            var metadata = PropertyUtils.GetValue<InventoryMetadata>(property, fieldInfo);

            EditorGUI.LabelField(position, nameof(InventoryMetadata), "The basic implementation");

            EditorGUI.EndProperty();
        }
    }
}