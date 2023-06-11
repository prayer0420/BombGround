using Inventory.Scripts.Inventory.Items;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Inventory.Displays.PrefabFiller
{
    public class DisplayFiller : MonoBehaviour
    {
        [Header("Content")] [SerializeField] private RectTransform header;
        [SerializeField] private TMP_Text text;

        [Header("Grid Parent")] [SerializeField]
        private RectTransform gridParent;

        public RectTransform GridParent => gridParent;
        public ItemTable CurrentInventoryItem => _currentInventoryItem;

        private ItemTable _currentInventoryItem;
        private RectTransform _transform;

        private bool _initialRefreshUiDone;

        private void Awake()
        {
            _transform = GetComponent<RectTransform>();
        }

        public void Set(ItemTable itemTable)
        {
            _currentInventoryItem = itemTable;
            
            var anchoredPosition = new Vector2(0, 1);
            _transform.anchoredPosition = anchoredPosition;
            _transform.anchorMin = anchoredPosition;
            _transform.anchorMax = anchoredPosition;
            _transform.pivot = anchoredPosition;

            text.SetText(_currentInventoryItem.ItemDataSo.DisplayName);
            SetAnchorsProps();
            _transform.SetAsLastSibling();
        }

        private void SetAnchorsProps()
        {
            header.pivot = new Vector2(0, 1);
            _transform.pivot = new Vector2(0, 1);
        }

        public void UnSet()
        {
            _currentInventoryItem = null;

            text.SetText("");
            _transform.SetAsLastSibling();
        }

        public void Sort()
        {
            _transform.SetAsFirstSibling();
        }

        public void Open()
        {
            gameObject.SetActive(true);
        }

        private void RefreshUI()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(header);
            LayoutRebuilder.ForceRebuildLayoutImmediate(_transform);
        }
    }
}