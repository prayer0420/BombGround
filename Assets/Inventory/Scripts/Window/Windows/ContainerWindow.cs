using Inventory.Scripts.Inventory.ItemsMetadata;
using UnityEngine;

namespace Inventory.Scripts.Window.Windows
{
    public class ContainerWindow : Window
    {
        [Header("Inspect Window Settings")] [SerializeField]
        private RectTransform content;

        private ContainerMetadata _containerMetadata;

        public const string KeyInventory = nameof(ContainerWindow);

        protected override void OnSetWindowProps()
        {
            SetTitle("Container " + CurrentItemTable.ItemDataSo.DisplayName);

            _containerMetadata = (ContainerMetadata)CurrentItemTable.InventoryMetadata;

            _containerMetadata.OpenInventory(KeyInventory, content);
        }

        protected override void OnCloseWindow()
        {
            base.OnCloseWindow();
            
            _containerMetadata.CloseInventory(KeyInventory);
        }
    }
}