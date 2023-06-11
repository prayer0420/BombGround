using Inventory.Scripts.ScriptableObjects.Events;
using Inventory.Scripts.ScriptableObjects.Events.Holders;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory.Scripts.Inventory.Holders
{
    [RequireComponent(typeof(ItemHolder))]
    public class ItemHolderInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private OnItemHolderInteractEventChannelSo onItemHolderInteractEventChannelSo;

        private ItemHolder _containerHolder;

        private void Awake()
        {
            _containerHolder = GetComponent<ItemHolder>();
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onItemHolderInteractEventChannelSo.RaiseEvent(_containerHolder);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onItemHolderInteractEventChannelSo.RaiseEvent(null);
        }
    }
}