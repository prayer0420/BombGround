using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects.Events.Holders;
using Inventory.Scripts.ScriptableObjects.Events.Options;
using Inventory.Scripts.ScriptableObjects.Events.Windows;
using UnityEngine;

namespace Inventory.Scripts.Window.Windows
{
    public class ContainerWindowManager : WindowManager
    {
        [Header("Listening on...")] [SerializeField]
        private OnItemExecuteOptionEventChannelSo onOpenItemEventChannelSo;

        [SerializeField] private WindowContainerChangeStateEventChannelSo windowContainerChangeStateEventChannelSo;

        [SerializeField] private OnEquipItemTableInHolderEventChannelSo onEquipItemContainerInHolderEventChannelSo;

        private void OnEnable()
        {
            onOpenItemEventChannelSo.OnEventRaised += OpenContainerAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised += ChangeStateFromWindow;
            onEquipItemContainerInHolderEventChannelSo.OnEventRaised += CloseContainerOnEquip;
        }

        private void OnDisable()
        {
            onOpenItemEventChannelSo.OnEventRaised -= OpenContainerAbstractItem;
            windowContainerChangeStateEventChannelSo.OnEventRaised -= ChangeStateFromWindow;
            onEquipItemContainerInHolderEventChannelSo.OnEventRaised -= CloseContainerOnEquip;
        }

        private void OpenContainerAbstractItem(AbstractItem abstractItem)
        {
            OpenWindow(abstractItem.ItemTable);
        }

        private void ChangeStateFromWindow(ItemTable itemTable, WindowUpdateState windowUpdateState)
        {
            if (WindowUpdateState.Open == windowUpdateState)
            {
                OpenWindow(itemTable);
                return;
            }

            if (WindowUpdateState.Close == windowUpdateState)
            {
                CloseWindow(itemTable);
            }
        }

        private void CloseContainerOnEquip(ItemTable itemTable, HolderInteractionStatus holderInteractionStatus)
        {
            if (HolderInteractionStatus.UnEquip == holderInteractionStatus) return;

            CloseWindow(itemTable);
        }

        protected override void CloseWindow(ItemTable itemTable)
        {
            if (itemTable == null) return;

            if (!itemTable.IsContainer()) return;

            base.CloseWindow(itemTable);
        }

        protected override Window GetPrefabWindow()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ContainerPrefabWindow;
        }

        protected override int GetMaxWindowOpen()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.MaxContainerWindowOpen;
        }
    }
}