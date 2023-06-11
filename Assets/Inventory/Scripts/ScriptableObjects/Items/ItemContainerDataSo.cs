using Inventory.Scripts.Inventory;
using Inventory.Scripts.Inventory.Grids;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Items
{
    [CreateAssetMenu(menuName = "Inventory/Items/Containers/New Item Container")]
    public class ItemContainerDataSo : ItemDataSo
    {
        [Header("Container Settings")]
        [SerializeField] private ContainerGrids containerGrids;

        public ContainerGrids ContainerGrids => containerGrids;
    }
}