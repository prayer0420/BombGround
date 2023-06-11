using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects.Events.Options;
using Inventory.Scripts.ScriptableObjects.Events.Windows;
using UnityEngine;

namespace Inventory.Scripts.Window.Windows
{
    public class InspectWindowManager : WindowManager
    {
        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onInspectItemEventChannelSo;

        [SerializeField] private WindowContainerChangeStateEventChannelSo windowContainerChangeStateEventChannelSo;

        private void OnEnable()
        {
            onInspectItemEventChannelSo.OnEventRaised += InspectAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised += HandleChangeStateWindow;
        }

        private void OnDisable()
        {
            onInspectItemEventChannelSo.OnEventRaised -= InspectAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised -= HandleChangeStateWindow;
        }

        private void InspectAbstractItem(AbstractItem abstractItem)
        {
            OpenWindow(abstractItem.ItemTable);
        }

        private void HandleChangeStateWindow(ItemTable itemTable, WindowUpdateState windowUpdateState)
        {
            if (windowUpdateState == WindowUpdateState.Open)
            {
                OpenWindow(itemTable);
                return;
            }

            if (windowUpdateState == WindowUpdateState.Close)
            {
                CloseWindow(itemTable);
            }
        }


        protected override Window GetPrefabWindow()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.InspectPrefabWindow;
        }

        protected override int GetMaxWindowOpen()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.MaxInspectWindowOpen;
        }
    }
}