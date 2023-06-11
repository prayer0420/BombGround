using Inventory.Scripts.ScriptableObjects.Audio;
using Inventory.Scripts.ScriptableObjects.Events;
using Inventory.Scripts.ScriptableObjects.Events.Audio;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Options
{
    [CreateAssetMenu(menuName = "Inventory/Options/New Audio Option")]
    public class AudioOptionSo : OptionSo
    {
        [Header("Audio Settings")]
        [SerializeField] private OnAudioStateEventChannelSo onAudioStateEventChannelSo;

        [SerializeField] private AudioStateSo audioStateSo;

        protected override void OnExecuteOption()
        {
            base.OnExecuteOption();
            
            onAudioStateEventChannelSo.RaiseEvent(audioStateSo);
        }
    }
}
