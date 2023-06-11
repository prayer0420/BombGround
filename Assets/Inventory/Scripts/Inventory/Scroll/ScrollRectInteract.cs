using Inventory.Scripts.ScriptableObjects.Events;
using Inventory.Scripts.ScriptableObjects.Events.Scroll;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Scripts.Inventory.Scroll
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollRectInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Broadcasting on...")]
        [SerializeField] private OnScrollRectInteractEventChannelSo onScrollRectInteractEventChannelSo;

        private ScrollRect _scrollRect;

        private void Awake()
        {
            _scrollRect = GetComponent<ScrollRect>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onScrollRectInteractEventChannelSo.RaiseEvent(_scrollRect);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onScrollRectInteractEventChannelSo.RaiseEvent(null);
        }
    }
}