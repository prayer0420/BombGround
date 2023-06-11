using Inventory.Scripts.Inventory.Items;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Scripts.Window
{
    public abstract class Window : MonoBehaviour, IPointerDownHandler
    {
        [Header("Window Settings")] [SerializeField]
        private Sprite icon;

        [SerializeField] private string text;

        [SerializeField] private Sprite closeIcon;

        [Header("Window Reference")] [SerializeField]
        private Image iconImage;

        [SerializeField] private TMP_Text title;

        [SerializeField] private Image closeIconImage;

        public RectTransform RectTransform { get; private set; }

        public ItemTable CurrentItemTable { get; private set; }

        private void Awake()
        {
            iconImage.sprite = icon;
            title.text = text;
            closeIconImage.sprite = closeIcon;

            RectTransform = GetComponent<RectTransform>();
        }

        protected void SetTitle(string newTitle)
        {
            title.SetText(newTitle);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            RectTransform.SetAsLastSibling();
        }

        public void SetItemTable(ItemTable itemTable)
        {
            CurrentItemTable = itemTable;
            OnSetWindowProps();
        }

        protected abstract void OnSetWindowProps();

        public void CloseWindow()
        {
            gameObject.SetActive(false);
            OnCloseWindow();
            CurrentItemTable = null;
        }

        protected virtual void OnCloseWindow()
        {
        }
    }
}