using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Holders;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.Utilities
{
    [DefaultExecutionOrder(100)]
    public class AddInitialItemToHolderUtility : MonoBehaviour
    {
        [Header("Configs")] [SerializeField] private ItemHolder itemHolder;

        [SerializeField] private ItemDataSo initialItemDataSo;

        [Header("Supplier")] [SerializeField] private InventorySupplierSo inventorySupplierSo;

        private void Start()
        {
            var (_, equippableMessages) = inventorySupplierSo.TryEquipItem(initialItemDataSo, itemHolder);

            if (equippableMessages == EquippableMessages.Equipped)
            {
                Debug.Log("Equipped initial item!");
            }
        }
    }
}