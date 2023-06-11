using Inventory.Scripts.Options;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Options
{
    [CreateAssetMenu(menuName = "Events/Options/On Options Interact Event ChannelSo")]
    public class OnOptionInteractEventChannelSo : ScriptableObject
    {
        public event UnityAction<OptionsController> OnEventRaised;

        public void RaiseEvent(OptionsController value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}
