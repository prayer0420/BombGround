using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.Inventory.ItemsMetadata;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Displays;
using UnityEngine;

namespace Inventory.Scripts.Inventory.Holders
{
    public class ContainerHolder : ItemHolder
    {
        [Space(24)] [Header("Container Holder Settings")] [SerializeField]
        private OnItemContainerInteractEventChannelSo onItemContainerInteractEventChannelSo;

        [Header("Inventory Settings")] [SerializeField]
        private InventorySo inventorySo;

        protected override void OnEquipItem(ItemTable equippedItemTable)
        {
            var containerMetadata = (ContainerMetadata)equippedItemTable.InventoryMetadata;

            containerMetadata.OnEquipContainerHolder();

            if (inventorySo != null)
            {
                inventorySo.AddInventory(equippedItemTable);
            }

            onItemContainerInteractEventChannelSo.RaiseEvent(equippedItemTable,
                InventoryInteractionStatus.Open);
        }

        protected override void OnUnEquipItem(ItemTable equippedItemTable)
        {
            if (inventorySo == null) return;

            inventorySo.RemoveInventory(equippedItemTable);

            onItemContainerInteractEventChannelSo.RaiseEvent(equippedItemTable,
                InventoryInteractionStatus.Close);
        }
    }
}