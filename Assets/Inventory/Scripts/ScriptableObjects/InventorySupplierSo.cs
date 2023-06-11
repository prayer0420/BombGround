using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Grids;
using Inventory.Scripts.Inventory.Holders;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects.Anchors;
using Inventory.Scripts.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects
{
    // [CreateAssetMenu(menuName = "Inventory/Supplier/InventorySupplierSo")]
    public class InventorySupplierSo : ScriptableObject
    {
        [Header("Inventory Settings Anchor So")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Abstract Grid Selected AnchorSo")] [SerializeField]
        private AbstractGridSelectedAnchorSo abstractGridSelectedAnchorSo;

        public (ItemTable, InventoryMessages) PlaceItem(ItemDataSo itemDataSo, GridTable gridTable)
        {
            var itemTable = new ItemTable(itemDataSo, abstractGridSelectedAnchorSo);

            var inserted = PlaceItem(itemTable, gridTable);

            return (itemTable, inserted);
        }

        public InventoryMessages PlaceItem(ItemTable itemTable, GridTable gridTable)
        {
            if (gridTable == null)
            {
                return InventoryMessages.NoGridTableSelected;
            }

            var posOnGrid = gridTable.FindSpaceForObjectAnyDirection(itemTable);

            if (posOnGrid == null)
            {
                return InventoryMessages.InventoryFull;
            }

            var inventoryMessage = gridTable.PlaceItem(itemTable, posOnGrid.Value.x, posOnGrid.Value.y);

            return inventoryMessage;
        }

        public void RemoveItem(ItemTable itemTable)
        {
            var currentGridTable = itemTable.CurrentGridTable;

            if (currentGridTable != null)
            {
                currentGridTable.RemoveItem(itemTable);
                return;
            }

            var currentItemHolder = itemTable.CurrentItemHolder;

            if (currentItemHolder != null)
            {
                currentItemHolder.UnEquip(itemTable);
            }
        }

        public (ItemTable, EquippableMessages) TryEquipItem(ItemDataSo itemDataSo, ItemHolder itemHolder)
        {
            var itemTable = new ItemTable(itemDataSo, abstractGridSelectedAnchorSo);

            if (itemHolder == null)
            {
                return (null, EquippableMessages.NoItemHolderSelected);
            }

            var equippableMessages = TryEquipItem(itemTable, itemHolder);

            return (itemTable, equippableMessages);
        }

        public EquippableMessages TryEquipItem(ItemTable itemTable, ItemHolder itemHolder)
        {
            return itemHolder == null ? EquippableMessages.NoItemHolderSelected : itemHolder.TryEquipItem(itemTable);
        }

        public ItemTable InitializeEnvironmentContainer(ItemContainerDataSo itemContainerDataSo,
            Transform environmentGoTransform)
        {
            var itemPrefab = GetItemPrefabSo().ItemPrefab;

            var instantiate = Instantiate(itemPrefab, environmentGoTransform);

            var abstractItem = instantiate.GetComponent<AbstractItem>();

            abstractItem.Set(itemContainerDataSo);

            abstractItem.gameObject.SetActive(false);

            return abstractItem.ItemTable;
        }

        private ItemPrefabSo GetItemPrefabSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ItemPrefabSo;
        }
    }
}