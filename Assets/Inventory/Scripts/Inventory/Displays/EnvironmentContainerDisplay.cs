using System.Collections.Generic;
using Inventory.Scripts.Inventory.Holders;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.Inventory.ItemsMetadata;
using UnityEngine.UI;

namespace Inventory.Scripts.Inventory.Displays
{
    public class EnvironmentContainerDisplay : AbstractContainerDisplay<EnvironmentContainerHolder>
    {
        private const string KeyInventory = nameof(EnvironmentContainerDisplay);

        protected override void OnAddDisplayContainer(EnvironmentContainerHolder container)
        {
            base.OnAddDisplayContainer(container);

            OpenInventory(container);
        }

        protected override void OnRemoveDisplayContainer(EnvironmentContainerHolder container)
        {
            base.OnRemoveDisplayContainer(container);

            CloseInventory(container);
        }

        private void OpenInventory(EnvironmentContainerHolder containerHolder)
        {
            var itemTable = containerHolder.GetItemTable();

            if (!itemTable.IsContainer()) return;

            var displayFiller = TryGetPrefab(itemTable);

            if (displayFiller == null) return;

            var containerMetadata = (ContainerMetadata)itemTable.InventoryMetadata;

            containerMetadata.OpenInventory(KeyInventory, displayFiller.GridParent);

            displayFiller.gameObject.SetActive(true);
        }

        private void CloseInventory(EnvironmentContainerHolder environmentContainerHolder)
        {
            if (environmentContainerHolder == null) return;

            var itemTable = environmentContainerHolder.GetItemTable();

            if (!itemTable.IsContainer()) return;

            var containerMetadata = (ContainerMetadata)itemTable.InventoryMetadata;

            containerMetadata.CloseInventory(KeyInventory);

            RemoveDisplayFiller(itemTable);
        }

        protected override List<ItemTable> GetAllInventoryItems()
        {
            return GetDisplayedContainers().ConvertAll(input => input.GetItemTable());
        }
    }
}