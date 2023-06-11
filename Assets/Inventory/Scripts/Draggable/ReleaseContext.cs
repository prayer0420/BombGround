using Inventory.Scripts.Inventory.Grids;
using Inventory.Scripts.Inventory.Holders;
using Inventory.Scripts.ScriptableObjects.Tile;

namespace Inventory.Scripts.Draggable
{
    public class ReleaseContext
    {
        public bool Debug { get; set; }

        public PickupState PickupState { get; }
        public AbstractGrid SelectedAbstractGrid { get; }
        public ItemHolder SelectedItemHolder { get; }
        public TileGridHelperSo TileGridHelperSo { get; }

        public ReleaseContext(PickupState pickupState, AbstractGrid selectedAbstractGrid,
            ItemHolder selectedItemHolder, TileGridHelperSo tileGridHelperSo)
        {
            PickupState = pickupState;
            SelectedAbstractGrid = selectedAbstractGrid;
            SelectedItemHolder = selectedItemHolder;
            TileGridHelperSo = tileGridHelperSo;
        }
    }
}