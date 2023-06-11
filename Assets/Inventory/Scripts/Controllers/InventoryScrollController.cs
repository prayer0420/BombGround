using Inventory.Scripts.Helper;
using Inventory.Scripts.Inputs;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Items;
using Inventory.Scripts.ScriptableObjects.Events.Scroll;
using Inventory.Scripts.ScriptableObjects.Tile;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Controllers
{
    public class InventoryScrollController : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Listening on...")] [SerializeField]
        private InputProviderSo inputProviderSo;

        [SerializeField] private OnAbstractItemBeingDragEventChannelSo onAbstractItemBeingDragEventChannelSo;

        [SerializeField] private OnScrollRectInteractEventChannelSo onScrollRectInteractEventChannelSo;

        private AbstractItem _currentInventoryItemBeingDrag;
        private ScrollRect _currentScrollRectSelected;

        private void OnEnable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised += ChangeAbstractItem;
            onScrollRectInteractEventChannelSo.OnEventRaised += ChangeScrollRect;
        }

        private void OnDisable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised -= ChangeAbstractItem;
            onScrollRectInteractEventChannelSo.OnEventRaised -= ChangeScrollRect;
        }

        private void ChangeAbstractItem(AbstractItem inventoryItem)
        {
            _currentInventoryItemBeingDrag = inventoryItem;
        }

        private void ChangeScrollRect(ScrollRect scrollRect)
        {
            _currentScrollRectSelected = scrollRect;
        }

        private void Update()
        {
            if (_currentInventoryItemBeingDrag == null) return;

            if (_currentScrollRectSelected == null) return;

            var tileGridHelperSo = GetTileGridHelperSo();

            var cursorPosition = inputProviderSo.GetState().CursorPosition;

            var pointerViewportPosition =
                tileGridHelperSo.GetLocalPosition(_currentScrollRectSelected.viewport, cursorPosition);

            if (pointerViewportPosition.y < _currentScrollRectSelected.viewport.rect.min.y + GetHoldScrollPadding())
            {
                var rect = _currentScrollRectSelected.viewport.rect;
                var scrollValue = _currentScrollRectSelected.verticalNormalizedPosition * rect.height;
                scrollValue -= GetHoldScrollRate() * Time.deltaTime;
                _currentScrollRectSelected.verticalNormalizedPosition = Mathf.Clamp01(scrollValue / rect.height);
            }

            if (pointerViewportPosition.y > _currentScrollRectSelected.viewport.rect.max.y - GetHoldScrollPadding())
            {
                var rect = _currentScrollRectSelected.viewport.rect;
                var scrollValue = _currentScrollRectSelected.verticalNormalizedPosition * rect.height;
                scrollValue += GetHoldScrollRate() * Time.deltaTime;
                _currentScrollRectSelected.verticalNormalizedPosition = Mathf.Clamp01(scrollValue / rect.height);
            }
        }

        private float GetHoldScrollRate()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.HoldScrollRate;
        }

        private float GetHoldScrollPadding()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.HoldScrollPadding;
        }

        private TileGridHelperSo GetTileGridHelperSo()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.TileGridHelperSo;
        }

        public void SetInventorySettingsAnchorSo(InventorySettingsAnchorSo inventorySettingsAnchor)
        {
            inventorySettingsAnchorSo = inventorySettingsAnchor;
        }
    }
}