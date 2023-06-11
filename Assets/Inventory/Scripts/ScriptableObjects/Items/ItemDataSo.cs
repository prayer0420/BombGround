using System.Linq;
using Inventory.Scripts.ScriptableObjects.Audio;
using Inventory.Scripts.ScriptableObjects.Items.Dimensions;
using Inventory.Scripts.ScriptableObjects.Items.IconSize;
using Inventory.Scripts.ScriptableObjects.Options;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/New Item")]
    public class ItemDataSo : ScriptableObject
    {
        [Header("Item Data Settings")] [SerializeField]
        private string displayName;

        [SerializeField] private DimensionsSo dimensionsSo;

        [SerializeField] private Sprite icon;

        [SerializeField] private ItemDataTypeSo itemDataTypeSo;

        [SerializeField] private AudioCueSo audioCueSo;

        [Header("Images Configuration")] [SerializeField] [Tooltip("The icon size in the grid")]
        private IconSizeSo iconSizeSo;

        [SerializeField] [Tooltip("The icon size when the item is in the ItemHolder")]
        private IconSizeSo inHolderIconSizeSo;

        [Header("InGame Prefab Configuration")] [SerializeField]
        private GameObject prefabInGame;

        [Header("Group Options Settings")] [SerializeField]
        private OptionSo[] optionsSo;

        public string DisplayName => displayName;

        public DimensionsSo DimensionsSo
        {
            get => dimensionsSo;
            set => dimensionsSo = value;
        }

        public Sprite Icon => icon;

        public ItemDataTypeSo ItemDataTypeSo => itemDataTypeSo;

        public IconSizeSo IconSizeSo => iconSizeSo;

        public IconSizeSo InHolderIconSizeSo => inHolderIconSizeSo;

        public GameObject PrefabInGame => prefabInGame;

        public AudioCueSo AudioCueSo => audioCueSo;

        public OptionSo[] OptionsSo
        {
            get => optionsSo;
            set => optionsSo = value;
        }

        public OptionSo[] GetOptionsOrdered()
        {
            return OptionsSo.OrderBy(so => so.Order).ToArray();
        }
    }
}