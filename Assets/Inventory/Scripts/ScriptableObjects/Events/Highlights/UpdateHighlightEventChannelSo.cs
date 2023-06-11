using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Highlights
{
    [CreateAssetMenu(menuName = "Events/Highlights/Update Highlight Event ChannelSo")]
    public class UpdateHighlightEventChannelSo : ScriptableObject
    {
        public event UnityAction OnEventRaised;

        public void RaiseEvent()
        {
            OnEventRaised?.Invoke();
        }
    }
}