using System;
using System.Linq;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.Inventory.ItemsMetadata;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Items;
using UnityEngine;

namespace Inventory.Scripts.Controllers
{
    public class InventoryHighlightSpaceContainerController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Player Inventory")] [SerializeField]
        private InventorySo playerInventorySo;

        [Header("Listening on...")] [SerializeField]
        private OnAbstractItemBeingDragEventChannelSo onAbstractItemBeingDragEventChannelSo;

        private void Awake()
        {
            playerInventorySo.Inventories.Clear();
        }

        private void OnEnable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised += HighlightBackpackWithSpace;
        }

        private void OnDisable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised -= HighlightBackpackWithSpace;
        }

        private void HighlightBackpackWithSpace(AbstractItem inventoryItem)
        {
            if (playerInventorySo.Inventories == null) return;

            if (!IsEnableHighlightContainerWithSpaceInPlayerInventory()) return;

            foreach (var item in from itemTable in playerInventorySo.Inventories
                     select (ContainerMetadata)itemTable.InventoryMetadata
                     into containerMetadata
                     select containerMetadata.GridsInventory
                     into grids
                     from gridTable in grids
                     select gridTable.GetAllItemsFromGrid()
                     into allItemsFromGrid
                     from item in allItemsFromGrid
                     where item != null
                     select item)
            {
                if (inventoryItem == null)
                {
                    item.UpdateUI(new ItemUIUpdater(
                        Tuple.Create<bool, Color?>(false, null)
                    ));
                    continue;
                }

                if (item.InventoryMetadata is not ContainerMetadata metadata) continue;

                var containsSpaceForItem = metadata.ContainsSpaceForItem(inventoryItem);

                if (inventoryItem.ItemTable.InventoryMetadata is ContainerMetadata itemDraggingMetadata)
                {
                    var isInsertingInsideYourself =
                        itemDraggingMetadata.IsInsertingInsideYourself(item.CurrentGridTable);

                    if (isInsertingInsideYourself)
                    {
                        continue;
                    }
                }

                // TODO: Validate if the inventoryItem is already inside the container that, if so should not update the background.

                item.UpdateUI(new ItemUIUpdater(
                    Tuple.Create<bool, Color?>(containsSpaceForItem,
                        GetColorOnHoverContainerThatCanBeInsertedInPlayerInventory())
                ));
            }
        }

        private bool IsEnableHighlightContainerWithSpaceInPlayerInventory()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.EnableHighlightContainerWithSpaceInPlayerInventory;
        }

        private Color GetColorOnHoverContainerThatCanBeInsertedInPlayerInventory()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.OnHoverContainerThatCanBeInsertedInPlayerInventory;
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }
    }
}