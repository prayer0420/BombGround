using Inventory.Scripts.Inventory.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Items
{
    [CreateAssetMenu(menuName = "Events/Items/On Abstract Item Being Drag Event ChannelSo")]
    public class OnAbstractItemBeingDragEventChannelSo : ScriptableObject
    {
        public event UnityAction<AbstractItem> OnEventRaised;

        public void RaiseEvent(AbstractItem value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}