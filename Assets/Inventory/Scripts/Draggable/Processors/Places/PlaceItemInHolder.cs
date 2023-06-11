using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Enums;
using UnityEngine;

namespace Inventory.Scripts.Draggable.Processors.Places
{
    // [CreateAssetMenu(menuName = "Inventory/Providers/Processors/Place Item in Holder Middleware")]
    public class PlaceItemInHolder : ReleaseProcessor
    {
        protected override void HandleProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedItemHolder = ctx.SelectedItemHolder;
            var selectedInventoryItem = ctx.PickupState.Item;
            var itemTable = selectedInventoryItem.ItemTable;

            var equippableMessages = selectedItemHolder.TryEquipItem(itemTable);

            if (ctx.Debug)
            {
                Debug.Log(("Placing item in holder. Holder: " + selectedItemHolder + " Status: " + equippableMessages)
                    .DraggableSystem());
            }

            finalState.Placed = equippableMessages == EquippableMessages.Equipped;
        }

        protected override bool ShouldProcess(ReleaseContext ctx, ReleaseState finalState)
        {
            var selectedItemHolder = ctx.SelectedItemHolder;

            return selectedItemHolder != null;
        }
    }
}