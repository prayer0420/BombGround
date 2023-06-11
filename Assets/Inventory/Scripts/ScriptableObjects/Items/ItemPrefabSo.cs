using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Items
{
    [CreateAssetMenu(menuName = "Inventory/Prefabs/New Item Prefab Config")]
    public class ItemPrefabSo : ScriptableObject
    {
        [SerializeField] private GameObject itemPrefab;

        public GameObject ItemPrefab => itemPrefab;
    }
}