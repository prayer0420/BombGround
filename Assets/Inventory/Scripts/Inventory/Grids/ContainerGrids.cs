using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Helper;
using UnityEngine;

namespace Inventory.Scripts.Inventory.Grids
{
    [RequireComponent(typeof(RectTransform))]
    public class ContainerGrids : MonoBehaviour
    {
        private void Awake()
        {
            gameObject.AddComponent<LockRotation>();
        }

        public AbstractGrid[] GetAbstractGrids()
        {
            return GetComponentsInChildren<AbstractGrid>();
        }

        public void SetGridTables(List<GridTable> existingItems)
        {
            var abstractGrids = GetAbstractGrids();

            foreach (var gridTable in existingItems)
            {
                // TODO: Improve this code...
                var abstractGrid = abstractGrids.First(grid =>
                    (grid.Grid == null || grid.Grid.Width == 0 && grid.Grid.Height == 0) &&
                    gridTable.Width == grid.GridWidth && gridTable.Height == grid.GridHeight);

                abstractGrid.Set(gridTable);
            }
        }
    }
}