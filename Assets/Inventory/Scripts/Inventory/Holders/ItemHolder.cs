using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Holders;
using Inventory.Scripts.ScriptableObjects.Events.Items;
using Inventory.Scripts.ScriptableObjects.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Inventory.Holders
{
    public class ItemHolder : MonoBehaviour
    {
        [SerializeField] private bool useDefaultSprite;

        [SerializeField] private Sprite defaultSprite;

        [SerializeField] private Color spriteColor;

        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Item Holder Settings")] [SerializeField]
        private ItemDataTypeSo itemDataTypeSo;

        [SerializeField] [Tooltip("Will display the inventory item icon rotated.")]
        private bool displayRotatedIcon;

        [Header("Listening on...")] [SerializeField]
        private OnAbstractItemBeingDragEventChannelSo onAbstractItemBeingDragEventChannelSo;

        [Header("Broadcasting on...")] [SerializeField]
        private OnEquipItemTableInHolderEventChannelSo onEquipItemTableInHolderEventChannelSo;

        [HideInInspector] public bool isEquipped;

        public bool UseDefaultSprite => useDefaultSprite;

        public Sprite DefaultSprite => defaultSprite;

        public Color SpriteColor => spriteColor;

        public ItemDataTypeSo ItemDataTypeSo => itemDataTypeSo;

        protected Image Image;
        protected Color DefaultColor;

        private Image _childedImage;

        private ItemTable _equippedItemTable;
        private AbstractItem _equippedAbstractItemUi;

        private RectTransform _holderRectTransform;
        private RectTransform _equippedInventoryItemRectTransform;

        private ItemTable _itemTableBeingDrag;
        private bool _shouldEmitEquipEvent;
        private AbstractItem _dragRepresentationInventoryItem;

        private void Awake()
        {
            Image = GetComponent<Image>();
            if (UseDefaultSprite)
            {
                _childedImage = GetChildedImage();
            }

            DefaultColor = Image.color;
            _holderRectTransform = GetComponent<RectTransform>();

            _dragRepresentationInventoryItem = GetOrInstantiateObjectIfNotExists();
        }

        private Image GetChildedImage()
        {
            try
            {
                var imageChilded = GetComponentsInChildren<Image>()[0];

                if (Image == imageChilded)
                {
                    imageChilded = GetComponentsInChildren<Image>()[1];
                }

                return imageChilded;
            }
            catch
            {
                Debug.LogError(
                    "Error getting childed image... Check if the image is created or 'useDefaultSprite' is unchecked."
                        .Settings());
                return null;
            }
        }

        protected virtual void OnEnable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised += ChangeAbstractItem;
        }

        protected virtual void OnDisable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised -= ChangeAbstractItem;
        }

        private void ChangeAbstractItem(AbstractItem abstractItem)
        {
            _itemTableBeingDrag = abstractItem != null ? abstractItem.ItemTable : null;
        }

        private void Update()
        {
            if (!GetShouldHighlightHolder()) return;

            if (_itemTableBeingDrag == null)
            {
                StopHighlight();
                return;
            }

            if (_equippedItemTable != null) return;

            HighlightItemHolder();
        }

        private void HighlightItemHolder()
        {
            if (!IsItemSameType(_itemTableBeingDrag)) return;

            Highlight();
        }

        protected bool IsItemSameType(ItemTable currentItemTable)
        {
            if (currentItemTable == null) return false;

            return itemDataTypeSo == currentItemTable.ItemDataSo.ItemDataTypeSo;
        }

        protected virtual void Highlight()
        {
            Image.color = GetColorHighlightHolder();
        }

        protected virtual void StopHighlight()
        {
            Image.color = DefaultColor;
        }

        protected virtual void OnEquipItem(ItemTable equippedItemTable)
        {
        }

        protected virtual void OnUnEquipItem(ItemTable equippedItemTable)
        {
        }

        public EquippableMessages TryEquipItem(ItemTable itemTable,
            bool emitEquipEvent = true)
        {
            if (itemTable == null) return EquippableMessages.Error;

            if (_equippedItemTable != null && isEquipped)
            {
                Debug.Log("Already equipped".Info());
                return EquippableMessages.AlreadyEquipped;
            }

            if (!IsItemSameType(itemTable))
            {
                return EquippableMessages.NotCorrectType;
            }

            _shouldEmitEquipEvent = emitEquipEvent;

            RemoveFromPreviousLocation(itemTable);

            itemTable.SetGridProps(null, default, default, this);

            var itemPrefab = GetItemPrefabSo().ItemPrefab;

            var gameObjectItem = Instantiate(itemPrefab);

            _equippedAbstractItemUi = gameObjectItem.GetComponent<AbstractItem>();

            var transformFromItem = _equippedAbstractItemUi.GetComponent<Transform>();

            transformFromItem.SetParent(transform);

            _equippedAbstractItemUi.RefreshItem(itemTable);

            return Equip(_equippedAbstractItemUi);
        }

        private static void RemoveFromPreviousLocation(ItemTable itemTable)
        {
            itemTable.CurrentGridTable?.RemoveItem(itemTable);
            if (itemTable.CurrentItemHolder != null)
            {
                itemTable.CurrentItemHolder.UnEquip(itemTable);
            }
        }

        private EquippableMessages Equip(AbstractItem selectedInventoryItem)
        {
            _equippedItemTable = selectedInventoryItem.ItemTable;
            _equippedAbstractItemUi = selectedInventoryItem;

            if (_equippedItemTable.IsRotated)
            {
                selectedInventoryItem.Rotate();
            }

            _equippedInventoryItemRectTransform = selectedInventoryItem.GetComponent<RectTransform>();

            DisableDefaultImage();
            ResizeImages(selectedInventoryItem, _equippedInventoryItemRectTransform);

            isEquipped = true;

            if (isEquipped && _shouldEmitEquipEvent)
            {
                PushBroadcastEvent(_equippedItemTable, HolderInteractionStatus.Equip);
            }

            OnEquipItem(_equippedItemTable);

            return EquippableMessages.Equipped;
        }

        private void ResizeImages(AbstractItem inventoryItem, RectTransform equippedInventoryItemRectTransform)
        {
            equippedInventoryItemRectTransform.SetParent(_holderRectTransform);

            var holderSize = GetSize();
            ResizeItem(equippedInventoryItemRectTransform, holderSize);

            inventoryItem.ResizeIconOnHolder(holderSize);

            if (displayRotatedIcon && !inventoryItem.ItemTable.IsRotated)
            {
                inventoryItem.Rotate();
            }
        }

        private void ResizeItem(RectTransform rectTransform, Vector2 size)
        {
            rectTransform.sizeDelta = size;

            var anchors = new Vector2(0.5f, 0.5f);

            rectTransform.anchorMin = anchors;
            rectTransform.anchorMax = anchors;
            rectTransform.pivot = anchors;

            rectTransform.localPosition = new Vector2(0, 0);
        }

        private Vector2 GetSize()
        {
            var sizeDelta = _holderRectTransform.sizeDelta;

            var size = new Vector2
            {
                x = displayRotatedIcon ? sizeDelta.y - 10 : sizeDelta.x - 10,
                y = displayRotatedIcon ? sizeDelta.x - 10 : sizeDelta.y - 10
            };

            return size;
        }

        public void UnEquip(ItemTable itemTable)
        {
            EnableDefaultImage();

            PushBroadcastEvent(itemTable, HolderInteractionStatus.UnEquip);

            if (itemTable == null) return;

            OnUnEquipItem(itemTable);

            _equippedItemTable = null;
            isEquipped = false;

            Destroy(_equippedAbstractItemUi.gameObject);
        }

        public AbstractItem PlaceDragRepresentation()
        {
            var inventoryItemDragRepresentation = GetOrInstantiateObjectIfNotExists();

            var itemDataSo = _equippedItemTable.ItemDataSo;

            inventoryItemDragRepresentation.SetDragProps(itemDataSo);
            inventoryItemDragRepresentation.ResizeIcon();

            var inventoryItemDragRepresentationRectTransform =
                inventoryItemDragRepresentation.GetComponent<RectTransform>();

            ResizeImages(inventoryItemDragRepresentation, inventoryItemDragRepresentationRectTransform);

            inventoryItemDragRepresentation.SetDragRepresentationDragStyle();

            inventoryItemDragRepresentation.gameObject.SetActive(true);

            isEquipped = false;
            return inventoryItemDragRepresentation;
        }

        private AbstractItem GetOrInstantiateObjectIfNotExists()
        {
            if (_dragRepresentationInventoryItem != null)
            {
                return _dragRepresentationInventoryItem;
            }

            var itemPrefab = GetItemPrefabSo().ItemPrefab;

            var instantiate = Instantiate(itemPrefab);
            instantiate.name += "_BeforeDragItemShowing";
            _dragRepresentationInventoryItem = instantiate.GetComponent<AbstractItem>();

            _dragRepresentationInventoryItem.Set(null, false);

            _dragRepresentationInventoryItem.transform.SetParent(gameObject.transform);

            instantiate.SetActive(false);

            return _dragRepresentationInventoryItem;
        }

        public void RemoveDragRepresentation()
        {
            UnEquip(_dragRepresentationInventoryItem.ItemTable);

            _dragRepresentationInventoryItem.gameObject.SetActive(false);
        }

        public AbstractItem GetItemEquipped()
        {
            if (isEquipped && _equippedItemTable != null)
            {
                return _equippedAbstractItemUi;
            }

            return null;
        }

        private void EnableDefaultImage()
        {
            if (_childedImage == null) return;

            _childedImage.gameObject.SetActive(true);
        }

        private void DisableDefaultImage()
        {
            if (_childedImage == null) return;

            _childedImage.gameObject.SetActive(false);
        }

        private ItemPrefabSo GetItemPrefabSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ItemPrefabSo;
        }

        private bool GetShouldHighlightHolder()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ShouldHighlightHolder;
        }

        private Color GetColorHighlightHolder()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ColorHighlightHolder;
        }

        private void PushBroadcastEvent(ItemTable itemTable, HolderInteractionStatus status)
        {
            if (onEquipItemTableInHolderEventChannelSo == null)
            {
                Debug.LogWarning("Not broadcasting event... The event is not configured...".Broadcasting());
                return;
            }

            onEquipItemTableInHolderEventChannelSo.RaiseEvent(itemTable, status);
        }
    }
}