using Inventory.Scripts.ScriptableObjects.Anchors;
using Inventory.Scripts.ScriptableObjects.Events.Grids;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Scripts.Inventory.Grids
{
    [RequireComponent(typeof(AbstractGrid))]
    public class GridInteract2D : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private OnGridInteractEventChannelSo onGridInteractEventChannelSo;
        [SerializeField] private AbstractGridSelectedAnchorSo abstractGridSelectedAnchorSo;

        private AbstractGrid _abstractGrid;

        private void Awake()
        {
            _abstractGrid = GetComponent<AbstractGrid>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            abstractGridSelectedAnchorSo.Value = _abstractGrid;
            onGridInteractEventChannelSo.RaiseEvent(_abstractGrid);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            abstractGridSelectedAnchorSo.Value = null;
            onGridInteractEventChannelSo.RaiseEvent(null);
        }
    }
}