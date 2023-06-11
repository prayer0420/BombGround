using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Options;
using Inventory.Scripts.ScriptableObjects.Events.Windows;
using UnityEngine;

namespace Inventory.Scripts.Options.Implementations
{
    public class DiscardManager : MonoBehaviour
    {
        [Header("Supplier")] [SerializeField] private InventorySupplierSo inventorySupplierSo;

        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onItemExecuteOptionEventChannelSo;

        [Header("Broadcasting on...")] [SerializeField]
        private WindowContainerChangeStateEventChannelSo windowContainerChangeStateEventChannelSo;

        private void OnEnable()
        {
            onItemExecuteOptionEventChannelSo.OnEventRaised += HandleDiscardOption;
        }

        private void OnDisable()
        {
            onItemExecuteOptionEventChannelSo.OnEventRaised -= HandleDiscardOption;
        }

        private void HandleDiscardOption(AbstractItem item)
        {
            var itemTable = item.ItemTable;

            inventorySupplierSo.RemoveItem(itemTable);

            windowContainerChangeStateEventChannelSo.RaiseEvent(itemTable, WindowUpdateState.Close);

            Destroy(item.gameObject);
        }
    }
}