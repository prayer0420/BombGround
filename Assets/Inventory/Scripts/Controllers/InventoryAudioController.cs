using Inventory.Scripts.ScriptableObjects.Audio;
using Inventory.Scripts.ScriptableObjects.Events;
using Inventory.Scripts.ScriptableObjects.Events.Audio;
using UnityEngine;

namespace Inventory.Scripts.Controllers
{
    public class InventoryAudioController : MonoBehaviour
    {
        [Header("Dependencies")] [SerializeField]
        private AudioSource audioSource;

        [Header("Listening on...")] [SerializeField]
        private OnAudioStateEventChannelSo onAudioStateEventChannelSo;

        private void OnEnable()
        {
            onAudioStateEventChannelSo.OnEventRaised += PlayAudio;
        }

        private void OnDisable()
        {
            onAudioStateEventChannelSo.OnEventRaised -= PlayAudio;
        }

        private void PlayAudio(AudioStateSo audioStateSo)
        {
            if (audioStateSo == null) return;

            var audioSo = audioStateSo.GetAudioSo();

            if (audioSo == null) return;

            audioSource.mute = audioSo.Mute;
            audioSource.pitch = audioSo.Pitch;
            audioSource.volume = audioSo.Volume;
            audioSource.clip = audioSo.Clip;
            audioSource.Play();
        }
    }
}