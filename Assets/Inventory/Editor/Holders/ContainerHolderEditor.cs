using Inventory.Editor.Helper;
using Inventory.Scripts.Inventory;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor
{
    // [CustomEditor(typeof(OldHolder))]
    public class ContainerHolderEditor : UnityEditor.Editor

    {
        // public override void OnInspectorGUI()
        // {
        //     DrawDefaultInspector();
        //
        //     var inventoryGrid = (OldHolder) target;
        //
        //     inventoryGrid.shouldDisplayName = DrawEditorHelper.CreateToggleProperty(inventoryGrid.shouldDisplayName);
        //
        //     BuildGridNameOptions(inventoryGrid);
        //     
        //     EditorGUILayout.Space(15);
        //     DrawEditorHelper.DrawDefaultHeader("Container Grids - Helper");
        //     if (GUILayout.Button("Apply Container Grids"))
        //     {
        //         inventoryGrid.ApplyContainerGrids();
        //     }
        // }
        //
        // private static void BuildGridNameOptions(OldHolder oldHolder)
        // {
        //     if (!oldHolder.shouldDisplayName)
        //     {
        //         oldHolder.DestroyInventoryGridName();
        //         return;
        //     }
        //
        //     EditorGUILayout.Space(15);
        //     DrawEditorHelper.DrawDefaultHeader("Container Grids Settings");
        //
        //     oldHolder.inventoryGridName =
        //         (TMP_Text) EditorGUILayout.ObjectField(new GUIContent("Container Grids Name"),
        //             oldHolder.inventoryGridName, typeof(TMP_Text), true);
        //
        //     EditorGUILayout.Space();
        //     if (GUILayout.Button("Create Name Area"))
        //     {
        //         oldHolder.CreateInventoryGridName();
        //     }
        // }
    }
}