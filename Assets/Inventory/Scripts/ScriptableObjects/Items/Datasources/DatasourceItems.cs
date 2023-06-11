using System.Collections.Generic;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Items.Datasources
{
    [CreateAssetMenu(menuName = "Inventory/Items/Datasource/New Items Datasource")]
    public class DatasourceItems : ScriptableObject
    {
        [Header("Items DataSource")] [SerializeField]
        private List<ItemDataSo> items = new();

        public ItemDataSo GetRandomItem()
        {
            var selectedItemId = Random.Range(0, items.Count);

            return items[selectedItemId];
        }

        public void Import(List<ItemDataSo> itemDataSos, bool replaceAllItemsInDatasource = true)
        {
            if (replaceAllItemsInDatasource)
            {
                items.Clear();
                items.AddRange(itemDataSos);
                return;
            }

            items.AddRange(itemDataSos);
        }
    }
}