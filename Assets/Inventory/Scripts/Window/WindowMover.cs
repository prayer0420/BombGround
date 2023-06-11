using Inventory.Scripts.ScriptableObjects.Anchors;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Scripts.Window
{
    public class WindowMover : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        [SerializeField] private CanvasAnchorSo canvasAnchorSo;

        private RectTransform _rectTransformFromParent;

        private void Start()
        {
            var windowInParent = GetComponentInParent<Window>();
            _rectTransformFromParent = windowInParent.GetComponent<RectTransform>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            _rectTransformFromParent.anchoredPosition += eventData.delta / canvasAnchorSo.Value.scaleFactor;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _rectTransformFromParent.SetAsLastSibling();
        }
    }
}