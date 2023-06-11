using Inventory.Scripts.Inventory.Items;
using UnityEngine;
using UnityEngine.Events;

namespace Inventory.Scripts.ScriptableObjects.Events.Windows
{
    public enum WindowUpdateState
    {
        Open,
        Close
    }
    
    /**
     * This will trigger for both windows, Inspect and Container... If you want to for specific window,
     * you can add new parameter in the UnityAction. And validate in the InspectWindowManager or ContainerWindowManager.
     */
    [CreateAssetMenu(menuName = "Events/Windows/Window Container Change State Event ChannelSo")]
    public class WindowContainerChangeStateEventChannelSo : ScriptableObject
    {
        public event UnityAction<ItemTable, WindowUpdateState> OnEventRaised;

        public void RaiseEvent(ItemTable itemTable, WindowUpdateState updateState)
        {
            OnEventRaised?.Invoke(itemTable, updateState);
        }
    }
}
