using Inventory.Scripts.Inventory.Grids;
using Inventory.Scripts.Inventory.Holders;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects.Tile;

namespace Inventory.Scripts.Draggable
{
    public class PickupContext
    {
        public bool Debug { get; set; }

        public AbstractItem SelectedInventoryItem { get; }
        public AbstractGrid AbstractGridFromStartDragging { get; }
        public ItemHolder ItemHolderFromStartDragging { get; }
        public TileGridHelperSo TileGridHelperSo { get; }

        public PickupContext(AbstractItem selectedInventoryItem, AbstractGrid abstractGridFromStartDragging,
            ItemHolder itemHolderFromStartDragging, TileGridHelperSo tileGridHelperSo)
        {
            SelectedInventoryItem = selectedInventoryItem;
            AbstractGridFromStartDragging = abstractGridFromStartDragging;
            ItemHolderFromStartDragging = itemHolderFromStartDragging;
            TileGridHelperSo = tileGridHelperSo;
        }
    }
}