using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Grids;
using UnityEditor;
using UnityEngine;

namespace Inventory.Editor.Grids
{
    [CustomEditor(typeof(AbstractGrid), true)]
    public class AbstractGridEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            var abstractGrid = (AbstractGrid)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Apply Grid"))
            {
                abstractGrid.ResizeGrid();

                var containerGrids = abstractGrid.GetComponentInParent<ContainerGrids>();

                if (containerGrids == null) return;

                HandleContainerGridsSize(containerGrids, abstractGrid);
            }
        }

        private static void HandleContainerGridsSize(ContainerGrids containerGrids, AbstractGrid itemGrid)
        {
            var abstractGrids = containerGrids.GetComponentsInChildren<AbstractGrid>();

            if (abstractGrids.Length == 1)
            {
                var rectTransform = itemGrid.GetComponent<RectTransform>();
                var containerGridsRectTransform = containerGrids.GetComponent<RectTransform>();

                containerGridsRectTransform.sizeDelta = rectTransform.sizeDelta;
                return;
            }

            Debug.Log(
                "You have multiples item grids inside the ContainerGrids. Make sure to fix the width and height from ContainerGrids in order to not bug the UI."
                    .Editor());
            // TODO: Handle width and height with multiple item grids.
        }
    }
}