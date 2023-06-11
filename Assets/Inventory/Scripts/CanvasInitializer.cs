using Inventory.Scripts.ScriptableObjects.Anchors;
using UnityEngine;

namespace Inventory.Scripts
{
    public class CanvasInitializer : MonoBehaviour
    {
        [SerializeField] private CanvasAnchorSo canvasAnchorSo;

        [SerializeField] private Canvas canvas;

        private void Awake()
        {
            canvasAnchorSo.Value = canvas;
        }

        public void SetAnchors(CanvasAnchorSo canvasAnchor)
        {
            canvas = GetComponent<Canvas>();
            canvasAnchorSo = canvasAnchor;
        }
    }
}