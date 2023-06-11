using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Holders
{
    [CreateAssetMenu(menuName = "Events/Holders/On Equip Item Table Event ChannelSo")]
    public class OnEquipItemTableInHolderEventChannelSo : ScriptableObject
    {
        public event UnityAction<ItemTable, HolderInteractionStatus> OnEventRaised;

        public void RaiseEvent(ItemTable value, HolderInteractionStatus holderInteractionStatus)
        {
            OnEventRaised?.Invoke(value, holderInteractionStatus);
        }
    }
}