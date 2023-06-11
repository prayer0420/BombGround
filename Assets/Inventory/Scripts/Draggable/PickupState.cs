using Inventory.Scripts.Inventory.Items;

namespace Inventory.Scripts.Draggable
{
    public class PickupState
    {
        public AbstractItem Item { get; set; }
        
        public bool? DraggableItemInitialRotation { get; set; }
    }
}