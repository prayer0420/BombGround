using Inventory.Scripts.Inventory.Displays.PrefabFiller;
using Inventory.Scripts.ScriptableObjects.Items;
using Inventory.Scripts.ScriptableObjects.Options;
using Inventory.Scripts.ScriptableObjects.Options.Configurations;
using Inventory.Scripts.ScriptableObjects.Tile;
using Inventory.Scripts.Window.Windows;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects
{
    [CreateAssetMenu(menuName = "Inventory/New Inventory Settings So")]
    public class InventorySettingsSo : ScriptableObject
    {
        [Header("Tile Settings")] [SerializeField] [Tooltip("The tile specification used calculate the size of tile.")]
        private TileSpecSo tileSpecSo;

        [Tooltip(
            "This TileGetGridPositionSo we use to keep the logic to calculate the grid position on the player click. You can implement your own, but inherit from it.")]
        [SerializeField]
        private TileGridHelperSo tileGridHelperSo;

        [Header("Grid Settings")] [SerializeField] [Tooltip("Will be the border of the grid.")]
        private Sprite borderGridSprite;

        [SerializeField] [Tooltip("Will be the slot icon from the grid tiles")]
        private Sprite inventorySlotSprite;

        [Header("Inventory Settings")]
        [SerializeField]
        [Tooltip("If false, doesn't need to hold mouse button in order to drag the item.")]
        private bool shouldHoldToDrag = true;

        [SerializeField]
        [Tooltip("Bool value if should display a item representation on last position when dragging...")]
        private bool shouldShowDragRepresentation = true;

        [SerializeField] [Tooltip("The prefab used to instantiate the item in the grid or in item holder.")]
        private ItemPrefabSo itemPrefabSo;

        [SerializeField] [Tooltip("Will be the prefab to be displayed when equip or open an inventory on the scene.")]
        private DisplayFiller displayFiller;

        [Header("Scroll Settings")]
        [SerializeField]
        [Tooltip("How close the item should be from corners of the ScrollArea in order to auto scroll in a direction.")]
        private float holdScrollPadding = 85;

        [SerializeField] [Tooltip("The speed of the scroll when is auto scrolling in a direction.")]
        private float holdScrollRate = 425;

        [Header("Options Configuration Settings")] [SerializeField]
        private OptionsConfigurationSo optionsConfigurationSo;

        [Header("Highlight Colors")] [SerializeField] [Tooltip("Show when the cursor is on top of a item in the grid.")]
        private Color onHoverItem;

        [SerializeField] [Tooltip("Show when a item is over another item in the grid. Cannot be inserted.")]
        private Color onHoverItemOverlapError;

        [SerializeField] [Tooltip("Show when the item can be inserted in the place.")]
        private Color onHoverEmptyCell;

        [SerializeField] [Tooltip("Show when a item is over a free space container.")]
        private Color onHoverContainerThatCanBeInserted;

        [Header("Highlight Item Holder")]
        [SerializeField]
        [Tooltip("Item Holder should be highlighted when a correct item holder is being drag.")]
        private bool shouldHighlightHolder = true;

        [SerializeField] [Tooltip("Color will be the borders of the Item Holder")]
        private Color colorHighlightHolder;

        [Header("Highlight Player Settings")]
        [SerializeField]
        [Tooltip("Will enable the highlight in containers with free space in player's inventory")]
        private bool enableHighlightContainerWithSpaceInPlayerInventory = true;

        [SerializeField]
        [Tooltip(
            "Will be the background color of all containers with free space in the player inventory (Player inventory is controlled by InventorySo)")]
        private Color onHoverContainerThatCanBeInsertedInPlayerInventory;

        [Header("Inspect Window Settings")] [SerializeField] [Tooltip("How many windows of inspect can be opened.")]
        private int maxInspectWindowOpen = 4;

        [SerializeField] [Tooltip("The prefab used when open a inspect window")]
        private InspectWindow inspectPrefabWindow;

        [Header("Container Window Settings")] [SerializeField] [Tooltip("How many windows of container can be opened.")]
        private int maxContainerWindowOpen = 4;

        [SerializeField] [Tooltip("The prefab used when open a container window")]
        private ContainerWindow containerPrefabWindow;

        public TileSpecSo TileSpecSo => tileSpecSo;

        public TileGridHelperSo TileGridHelperSo => tileGridHelperSo;

        public Sprite BorderGridSprite => borderGridSprite;

        public Sprite InventorySlotSprite => inventorySlotSprite;

        public bool ShouldHoldToDrag => shouldHoldToDrag;

        public bool ShouldShowDragRepresentation => shouldShowDragRepresentation;

        public ItemPrefabSo ItemPrefabSo => itemPrefabSo;

        public DisplayFiller DisplayFiller => displayFiller;

        public float HoldScrollPadding => holdScrollPadding;

        public float HoldScrollRate => holdScrollRate;

        public OptionsConfigurationSo OptionsConfigurationSo => optionsConfigurationSo;

        public Color OnHoverItem => onHoverItem;

        public Color OnHoverItemOverlapError => onHoverItemOverlapError;

        public Color OnHoverEmptyCell => onHoverEmptyCell;

        public Color OnHoverContainerThatCanBeInserted => onHoverContainerThatCanBeInserted;

        public bool ShouldHighlightHolder => shouldHighlightHolder;

        public Color ColorHighlightHolder => colorHighlightHolder;

        public bool EnableHighlightContainerWithSpaceInPlayerInventory =>
            enableHighlightContainerWithSpaceInPlayerInventory;

        public Color OnHoverContainerThatCanBeInsertedInPlayerInventory =>
            onHoverContainerThatCanBeInsertedInPlayerInventory;

        public int MaxInspectWindowOpen => maxInspectWindowOpen;

        public InspectWindow InspectPrefabWindow => inspectPrefabWindow;

        public int MaxContainerWindowOpen => maxContainerWindowOpen;

        public ContainerWindow ContainerPrefabWindow => containerPrefabWindow;
    }
}