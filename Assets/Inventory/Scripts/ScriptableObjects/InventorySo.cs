using System.Collections.Generic;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.Inventory.ItemsMetadata;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Inventory/New Inventory Group")]
    public class InventorySo : ScriptableObject
    {
        [SerializeReference] private List<ItemTable> inventories;

        public List<ItemTable> Inventories => inventories;

        public void AddInventory(ItemTable itemTable)
        {
            if (itemTable?.InventoryMetadata is not ContainerMetadata) return;

            inventories.Add(itemTable);
        }

        public void RemoveInventory(ItemTable inventoryItem)
        {
            inventories.Remove(inventoryItem);
        }
    }
}