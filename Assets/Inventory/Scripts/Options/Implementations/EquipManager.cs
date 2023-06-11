using System.Collections.Generic;
using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Holders;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Options;
using UnityEngine;

namespace Inventory.Scripts.Options.Implementations
{
    public class EquipManager : MonoBehaviour
    {
        [Header("Supplier")] [SerializeField] private InventorySupplierSo inventorySupplierSo;

        [Header("Holders")] [SerializeField] private List<ItemHolder> holders;

        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onItemExecuteEquipOptionEventChannelSo;

        private void OnEnable()
        {
            onItemExecuteEquipOptionEventChannelSo.OnEventRaised += HandleEquipOption;
        }

        private void OnDisable()
        {
            onItemExecuteEquipOptionEventChannelSo.OnEventRaised -= HandleEquipOption;
        }

        private void HandleEquipOption(AbstractItem item)
        {
            if (item == null) return;

            var itemTable = item.ItemTable;
            var itemDataTypeSo = itemTable.ItemDataSo.ItemDataTypeSo;

            var holder = holders.Find(itemHolder => !itemHolder.isEquipped && itemHolder.ItemDataTypeSo == itemDataTypeSo);

            if (holder == null)
            {
                Debug.Log("Not found any holder free and with the same item type. Type: " + itemDataTypeSo);
                return;
            }

            var equippableMessages = inventorySupplierSo.TryEquipItem(itemTable, holder);

            if (equippableMessages == EquippableMessages.Equipped)
            {
                Debug.Log("Equipped item!".Info());
            }
        }
    }
}