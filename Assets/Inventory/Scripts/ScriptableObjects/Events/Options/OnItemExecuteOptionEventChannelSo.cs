using Inventory.Scripts.Inventory.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Options
{
    [CreateAssetMenu(menuName = "Events/Options/On Inspect Inventory Item Event ChannelSo")]
    public class OnItemExecuteOptionEventChannelSo : ScriptableObject
    {
        public event UnityAction<AbstractItem> OnEventRaised;

        public void RaiseEvent(AbstractItem value)
        {
            OnEventRaised?.Invoke(value);
        }
    }
}