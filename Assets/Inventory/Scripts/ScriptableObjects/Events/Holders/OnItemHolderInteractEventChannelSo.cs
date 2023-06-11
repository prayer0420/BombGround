using Inventory.Scripts.Inventory.Holders;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Holders
{
    [CreateAssetMenu(menuName = "Events/Holders/On Item Holder Interact Event ChannelSo")]
    public class OnItemHolderInteractEventChannelSo : ScriptableObject
    {
        public event UnityAction<ItemHolder> OnEventRaised;

        public void RaiseEvent(ItemHolder value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}