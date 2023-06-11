using System.Collections.Generic;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.Inventory.ItemsMetadata;

namespace Inventory.Scripts.Inventory.Displays
{
    public class PlayerContainerDisplay : AbstractContainerDisplay<ItemTable>
    {
        private const string KeyInventory = nameof(PlayerContainerDisplay);

        protected override void OnAddDisplayContainer(ItemTable container)
        {
            base.OnAddDisplayContainer(container);

            var displayFiller = TryGetPrefab(container);

            if (displayFiller == null) return;

            var inventoryMetadataContainer = (ContainerMetadata)container.InventoryMetadata;

            inventoryMetadataContainer.OpenInventory(KeyInventory, displayFiller.GridParent);

            displayFiller.Open();
        }

        protected override void OnRemoveDisplayContainer(ItemTable container)
        {
            base.OnRemoveDisplayContainer(container);

            if (container == null) return;

            var inventoryMetadata = (ContainerMetadata)container.InventoryMetadata;

            inventoryMetadata.CloseInventory(KeyInventory);

            RemoveDisplayFiller(container);
        }

        protected override List<ItemTable> GetAllInventoryItems()
        {
            return GetDisplayedContainers();
        }
    }
}