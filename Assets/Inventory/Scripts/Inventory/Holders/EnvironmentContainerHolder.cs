using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Displays;
using Inventory.Scripts.ScriptableObjects.Items;
using UnityEngine;

namespace Inventory.Scripts.Inventory.Holders
{
    public class EnvironmentContainerHolder : MonoBehaviour
    {
        [Header("Inventory Supplier So")] [SerializeField]
        private InventorySupplierSo inventorySupplierSo;

        [Header("Environment Container Holder Settings")] [SerializeField]
        private ItemContainerDataSo itemContainerDataSo;

        [Header("Broadcasting on...")] [SerializeField]
        private OnEnvironmentContainerInteractEventChannelSo onEnvironmentContainerInteractEventChannelSo;

        private bool _isOpen;
        private ItemTable _containerInventoryItem;

        private void Start()
        {
            InitializeEnvironmentContainer();
        }

        private void InitializeEnvironmentContainer()
        {
            _containerInventoryItem =
                inventorySupplierSo.InitializeEnvironmentContainer(itemContainerDataSo, transform);
        }

        public void ToggleEnvironmentContainer()
        {
            if (_isOpen)
            {
                onEnvironmentContainerInteractEventChannelSo.RaiseEvent(this, InventoryInteractionStatus.Close);
                _isOpen = false;
                return;
            }

            onEnvironmentContainerInteractEventChannelSo.RaiseEvent(this, InventoryInteractionStatus.Open);
            _isOpen = true;
        }

        public ItemTable GetItemTable()
        {
            return _containerInventoryItem;
        }
    }
}