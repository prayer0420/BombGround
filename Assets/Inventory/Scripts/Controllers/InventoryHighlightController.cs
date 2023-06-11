using Inventory.Scripts.Inputs;
using Inventory.Scripts.Inventory.Grids;
using Inventory.Scripts.Inventory.Grids.Helper;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.Inventory.ItemsMetadata;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Anchors;
using Inventory.Scripts.ScriptableObjects.Events.Grids;
using Inventory.Scripts.ScriptableObjects.Events.Highlights;
using Inventory.Scripts.ScriptableObjects.Events.Items;
using Inventory.Scripts.ScriptableObjects.Tile;
using UnityEngine;

namespace Inventory.Scripts.Controllers
{
    public class InventoryHighlightController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [SerializeField] private HighlighterAnchorSo highlighterAnchorSo;

        [Header("Listening on...")] [SerializeField]
        private UpdateHighlightEventChannelSo updateHighlightEventChannelSo;

        [SerializeField] private OnGridInteractEventChannelSo onGridInteractEventChannelSo;

        [SerializeField] private OnAbstractItemBeingDragEventChannelSo onAbstractItemBeingDragEventChannelSo;

        [SerializeField] private InputProviderSo inputProviderSo;


        private RectTransform _highlightParent;

        private AbstractGrid _selectedAbstractGrid;
        private AbstractItem _selectedInventoryItem;

        private AbstractItem _itemToHighlight;
        private Vector2Int _oldPositionOnGrid;
        private bool? _itemRotation;

        private bool _shouldUpdateAgainWhenEnterAnotherGrid;

        private AbstractItem _hoveredContainerWithSpace;

        private void OnEnable()
        {
            inputProviderSo.OnRotateItem += ChangeItemRotationBeingDrag;

            onGridInteractEventChannelSo.OnEventRaised += HandleChangeGrid;
            onAbstractItemBeingDragEventChannelSo.OnEventRaised += HandleItemBeingDrag;
            updateHighlightEventChannelSo.OnEventRaised += UpdateHighlightFromEvent;
        }

        private void OnDisable()
        {
            inputProviderSo.OnRotateItem -= ChangeItemRotationBeingDrag;

            onGridInteractEventChannelSo.OnEventRaised -= HandleChangeGrid;
            onAbstractItemBeingDragEventChannelSo.OnEventRaised -= HandleItemBeingDrag;
            updateHighlightEventChannelSo.OnEventRaised -= UpdateHighlightFromEvent;
        }

        private void Update()
        {
            if (_selectedAbstractGrid == null && _hoveredContainerWithSpace != null)
            {
                _hoveredContainerWithSpace.SetBackground(false);
            }

            if (_selectedInventoryItem == null && _hoveredContainerWithSpace != null)
            {
                _hoveredContainerWithSpace.SetBackground(false);
            }

            if (_selectedAbstractGrid == null)
            {
                Show(false);
                return;
            }

            var tileGridHelperSo = GetTileGridHelperSo();

            var positionOnGrid =
                tileGridHelperSo.GetTileGridPosition(_selectedAbstractGrid.transform, _selectedInventoryItem);

            if (_oldPositionOnGrid == positionOnGrid && !_itemRotation.HasValue)
            {
                if (!_shouldUpdateAgainWhenEnterAnotherGrid) return;

                _shouldUpdateAgainWhenEnterAnotherGrid = false;
                UpdateHighlight(_oldPositionOnGrid);

                return;
            }

            UpdateHighlight(positionOnGrid);
        }

        private void UpdateHighlightFromEvent()
        {
            if (_selectedAbstractGrid == null) return;

            UpdateHighlight(_oldPositionOnGrid);
        }

        private void UpdateHighlight(Vector2Int positionOnGrid)
        {
            _oldPositionOnGrid = positionOnGrid;
            _itemRotation = null;

            if (_selectedInventoryItem == null)
            {
                _itemToHighlight = _selectedAbstractGrid.Grid.GetItem(positionOnGrid.x, positionOnGrid.y)
                    ?.GetAbstractItem();

                if (_itemToHighlight != null)
                {
                    Show(true);
                    SetDefaultColor();
                    SetSize(_itemToHighlight);
                    SetPosition(_itemToHighlight);
                }
                else
                {
                    Show(false);
                }

                return;
            }

            if (_selectedAbstractGrid == null) return;

            Show(true);
            SetColorOnDrag(_selectedInventoryItem, positionOnGrid);
            SetSize(_selectedInventoryItem);
            SetPosition(_selectedInventoryItem, positionOnGrid.x,
                positionOnGrid.y);
        }

        private void HandleChangeGrid(AbstractGrid abstractGrid)
        {
            _selectedAbstractGrid = abstractGrid;

            if (_selectedAbstractGrid != null) SetParent(_selectedAbstractGrid);

            _shouldUpdateAgainWhenEnterAnotherGrid = true;
        }

        private void ChangeItemRotationBeingDrag()
        {
            if (_selectedInventoryItem == null)
            {
                _itemRotation = null;
                return;
            }

            _itemRotation = _selectedInventoryItem.ItemTable.IsRotated;
        }

        private void HandleItemBeingDrag(AbstractItem inventoryItem)
        {
            _selectedInventoryItem = inventoryItem;
        }

        private void Show(bool shouldShow)
        {
            var highlighter = highlighterAnchorSo.Value;

            highlighter.Show(shouldShow);
        }

        private void SetDefaultColor()
        {
            var highlighter = highlighterAnchorSo.Value;

            highlighter.SetColor(GetOnHoverItem());
        }

        private void SetColorOnDrag(AbstractItem selectedInventoryItem, Vector2Int positionOnGrid)
        {
            var boundaryCheck = _selectedAbstractGrid.Grid.BoundaryCheck(
                positionOnGrid.x, positionOnGrid.y,
                selectedInventoryItem.ItemTable.Width,
                selectedInventoryItem.ItemTable.Height
            );

            var overlapCheck = false;

            if (boundaryCheck)
            {
                overlapCheck = _selectedAbstractGrid.Grid.OverlapCheck(positionOnGrid.x, positionOnGrid.y,
                    selectedInventoryItem.ItemTable.Width,
                    selectedInventoryItem.ItemTable.Height);
            }

            if (_hoveredContainerWithSpace != null)
            {
                _hoveredContainerWithSpace.SetBackground(false);
            }

            var highlighter = highlighterAnchorSo.Value;

            if (HandleContainerSpaceColor())
            {
                if (_hoveredContainerWithSpace == null) return;

                _hoveredContainerWithSpace.SetBackground(true, GetOnHoverContainerThatCanBeInserted());
                highlighter.SetColor(new Color(0, 0, 0, 0));
                return;
            }

            if (!overlapCheck)
            {
                highlighter.SetColor(GetOnHoverItemOverlap());
                return;
            }

            if (_selectedAbstractGrid.Grid.IsInsertingInsideYourself(selectedInventoryItem.ItemTable))
            {
                highlighter.SetColor(GetOnHoverItemOverlap());
                return;
            }

            highlighter.SetColor(GetOnHoverEmptyCell());
        }

        private bool HandleContainerSpaceColor()
        {
            var inventoryItem = GetItemHover(_selectedAbstractGrid);

            if (inventoryItem == null)
            {
                if (_hoveredContainerWithSpace == null) return false;

                _hoveredContainerWithSpace.SetBackground(false);
                _hoveredContainerWithSpace = null;

                return false;
            }

            _hoveredContainerWithSpace = inventoryItem;

            if (_selectedInventoryItem != null &&
                _selectedInventoryItem.ItemTable.InventoryMetadata is ContainerMetadata containerFromSelectedItem)
            {
                if (containerFromSelectedItem.IsInsertingInsideYourself(_hoveredContainerWithSpace.ItemTable
                        .CurrentGridTable))
                {
                    return false;
                }
            }

            // TODO: Improve the container grids to calculate once... calculate the max dimension can be inserted (item width x item height), so here we can just validate if have space or not...
            return _hoveredContainerWithSpace.ItemTable.InventoryMetadata is ContainerMetadata containerMetadata &&
                   containerMetadata.ContainsSpaceForItem(_selectedInventoryItem);
        }

        private AbstractItem GetItemHover(AbstractGrid abstractGrid)
        {
            var tileGridHelperSo = GetTileGridHelperSo();

            var tileGridPositionByGrid = tileGridHelperSo.GetTileGridPositionByGridTable(abstractGrid.transform);

            return _selectedAbstractGrid.Grid.GetItem(tileGridPositionByGrid.x, tileGridPositionByGrid.y)
                ?.GetAbstractItem();
        }

        private void SetSize(AbstractItem targetItem)
        {
            var tileGridHelperSo = GetTileGridHelperSo();

            var size = tileGridHelperSo.GetItemSize(targetItem.ItemTable.Width, targetItem.ItemTable.Height);

            var highlighter = highlighterAnchorSo.Value;

            highlighter.SetSizeDelta(size);
        }

        private void SetPosition(AbstractItem targetItem)
        {
            var tileGridHelperSo = GetTileGridHelperSo();

            var pos = tileGridHelperSo.CalculatePositionOnGrid(
                targetItem,
                targetItem.ItemTable.OnGridPositionX,
                targetItem.ItemTable.OnGridPositionY
            );

            var highlighter = highlighterAnchorSo.Value;

            highlighter.SetLocalPosition(pos);

            _highlightParent.SetAsLastSibling();
        }

        private void SetPosition(AbstractItem targetItem, int posX, int posY)
        {
            var tileGridHelperSo = GetTileGridHelperSo();

            var calculatedPositionOnGrid = tileGridHelperSo.CalculatePositionOnGrid(
                targetItem,
                posX,
                posY
            );

            var highlighter = highlighterAnchorSo.Value;

            highlighter.SetLocalPosition(calculatedPositionOnGrid);
        }

        private void SetParent(AbstractGrid targetGrid)
        {
            if (targetGrid == null)
                return;

            _highlightParent = (RectTransform)targetGrid.GetOrInstantiateHighlightArea();

            var highlighter = highlighterAnchorSo.Value;

            highlighter.SetParent(_highlightParent);
        }

        private Color GetOnHoverItem()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.OnHoverItem;
        }

        private Color GetOnHoverContainerThatCanBeInserted()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.OnHoverContainerThatCanBeInserted;
        }

        private Color GetOnHoverItemOverlap()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.OnHoverItemOverlapError;
        }

        private Color GetOnHoverEmptyCell()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.OnHoverEmptyCell;
        }

        private TileGridHelperSo GetTileGridHelperSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileGridHelperSo;
        }

        public void SetHighlighterAnchorSo(HighlighterAnchorSo highlighterAnchor)
        {
            highlighterAnchorSo = highlighterAnchor;
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }
    }
}