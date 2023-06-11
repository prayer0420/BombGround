using Inventory.Scripts.Inventory.Enums;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Displays
{
    public class OnContainerInteractEventChannelSo<T> : ScriptableObject
    {
        public event UnityAction<T, InventoryInteractionStatus> OnEventRaised;

        public void RaiseEvent(T containerInteract,
            InventoryInteractionStatus interactionStatus)
        {
            OnEventRaised?.Invoke(containerInteract, interactionStatus);
        }
    }
}