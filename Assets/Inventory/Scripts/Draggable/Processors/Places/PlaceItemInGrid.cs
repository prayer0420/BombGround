﻿using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Enums;
using UnityEngine;

namespace Inventory.Scripts.Draggable.Processors.Places
{
    // [CreateAssetMenu(menuName = "Inventory/Providers/Processors/Place Item in Grid Middleware")]
    public class PlaceItemInGrid : ReleaseProcessor
    {
        protected override void HandleProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedAbstractGrid = ctx.SelectedAbstractGrid;
            var selectedInventoryItem = ctx.PickupState.Item;
            var itemTable = selectedInventoryItem.ItemTable;

            var gridTable = selectedAbstractGrid.Grid;

            var ctxTileGridPosition = GetTileGridPosition(ctx);

            if (!ctxTileGridPosition.HasValue)
            {
                finalState.Placed = false;
                return;
            }

            var posX = ctxTileGridPosition.Value.x;
            var posY = ctxTileGridPosition.Value.y;

            var inventoryMessages = gridTable.PlaceItem(itemTable, posX, posY);

            if (ctx.Debug)
            {
                Debug.Log(("Placing item in grid. Grid: " + selectedAbstractGrid + " Status: " + inventoryMessages)
                    .DraggableSystem());
            }

            finalState.Placed = inventoryMessages == InventoryMessages.Inserted;
        }

        private Vector2Int? GetTileGridPosition(ReleaseContext ctx)
        {
            var tileGridHelperSo = ctx.TileGridHelperSo;

            var selectedAbstractGrid = ctx.SelectedAbstractGrid;
            var selectedInventoryItem = ctx.PickupState.Item;

            if (selectedAbstractGrid == null) return null;

            return tileGridHelperSo.GetTileGridPosition(selectedAbstractGrid.transform, selectedInventoryItem);
        }

        protected override bool ShouldProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedAbstractGrid = ctx.SelectedAbstractGrid;

            return selectedAbstractGrid != null;
        }
    }
}