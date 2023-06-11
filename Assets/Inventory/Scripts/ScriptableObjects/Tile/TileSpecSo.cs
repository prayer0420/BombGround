using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Tile
{
    [CreateAssetMenu(menuName = "Inventory/Tile/New Tile Spec So")]
    public class TileSpecSo : ScriptableObject
    {
        [SerializeField] private int tileSizeWidth = 84;
        [SerializeField] private int tileSizeHeight = 84;

        public int TileSizeWidth => tileSizeWidth;

        public int TileSizeHeight => tileSizeHeight;
    }
}
