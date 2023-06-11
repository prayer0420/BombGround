using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Inventory.Scripts.ScriptableObjects.Events.Scroll
{
    [CreateAssetMenu(menuName = "Events/Scroll/On ScrollRect Interact Event ChannelSo")]
    public class OnScrollRectInteractEventChannelSo : ScriptableObject
    {
        public event UnityAction<ScrollRect> OnEventRaised;

        public void RaiseEvent(ScrollRect value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}