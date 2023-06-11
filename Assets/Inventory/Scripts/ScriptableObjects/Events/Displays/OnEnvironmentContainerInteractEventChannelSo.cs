using Inventory.Scripts.Inventory.Holders;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Events.Displays
{
    [CreateAssetMenu(menuName = "Events/Display/On Environment Container Interact Event ChannelSo")]
    public class
        OnEnvironmentContainerInteractEventChannelSo : OnContainerInteractEventChannelSo<EnvironmentContainerHolder>
    {
    }
}