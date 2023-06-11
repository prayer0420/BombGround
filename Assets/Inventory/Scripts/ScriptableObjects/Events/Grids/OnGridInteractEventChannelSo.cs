using Inventory.Scripts.Inventory.Grids;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Grids
{
    [CreateAssetMenu(menuName = "Events/Grids/On Grid Interact Event ChannelSo")]
    public class OnGridInteractEventChannelSo : ScriptableObject
    {
        public event UnityAction<AbstractGrid> OnEventRaised;

        public void RaiseEvent(AbstractGrid value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}