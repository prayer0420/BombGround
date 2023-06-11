using Inventory.Scripts.Inventory.Items;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Events.Displays
{
    [CreateAssetMenu(menuName = "Events/Display/On Item Container Interact EventChannelSo")]
    public class OnItemContainerInteractEventChannelSo : OnContainerInteractEventChannelSo<ItemTable>
    {
    }
}