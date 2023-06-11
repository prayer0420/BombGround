using Inventory.Scripts.ScriptableObjects.Audio;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Audio
{
    [CreateAssetMenu(menuName = "Events/Audio/On Audio State Event ChannelSo")]
    public class OnAudioStateEventChannelSo : ScriptableObject
    {
        public event UnityAction<AudioStateSo> OnEventRaised;

        public void RaiseEvent(AudioStateSo value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}
