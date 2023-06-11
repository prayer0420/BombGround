using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects
{
    // Should be created only one in the project. This will holds the 'InventorySettingsSo' in order to remove the dependencies from code.
    // [CreateAssetMenu(menuName = "Inventory/Runtime Anchors/New Inventory Settings Anchor So")]
    public class InventorySettingsAnchorSo : ScriptableObject
    {
        [Header("Inventory Settings So")] [SerializeField]
        private InventorySettingsSo inventorySettingsSo;

        public InventorySettingsSo InventorySettingsSo => inventorySettingsSo;

        public bool IsSet()
        {
            return InventorySettingsSo != null;
        }
    }
}