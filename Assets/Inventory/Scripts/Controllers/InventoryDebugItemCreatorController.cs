using Inventory.Scripts.Helper;
using Inventory.Scripts.Inputs;
using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Grids;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Grids;
using Inventory.Scripts.ScriptableObjects.Items.Datasources;
using UnityEngine;

namespace Inventory.Scripts.Controllers
{
    public class InventoryDebugItemCreatorController : MonoBehaviour
    {
        [Header("Creator Configuration")] [SerializeField]
        private bool isEnabled;

        [Header("Datasource Items")] [SerializeField]
        private DatasourceItems datasourceItems;

        [Header("Inventory Supplier")] [SerializeField]
        private InventorySupplierSo inventorySupplierSo;

        [Header("Listening on...")] [SerializeField]
        private InputProviderSo inputProviderSo;

        [SerializeField] private OnGridInteractEventChannelSo onGridInteractEventChannelSo;

        private GridTable _selectedGridTable;

        private void Awake()
        {
            enabled = isEnabled;
        }

        private void OnEnable()
        {
            inputProviderSo.OnGenerateItem += InsertRandomItem;
            onGridInteractEventChannelSo.OnEventRaised += ChangeAbstractGrid;
        }

        private void OnDisable()
        {
            inputProviderSo.OnGenerateItem -= InsertRandomItem;
            onGridInteractEventChannelSo.OnEventRaised -= ChangeAbstractGrid;
        }

        private void ChangeAbstractGrid(AbstractGrid abstractGrid)
        {
            _selectedGridTable = abstractGrid != null ? abstractGrid.Grid : null;
        }

        private void InsertRandomItem()
        {
            if (_selectedGridTable == null) return;

            var itemDataSo = datasourceItems.GetRandomItem();

            var (itemTable, inserted) = inventorySupplierSo.PlaceItem(itemDataSo, _selectedGridTable);

            if (inserted.Equals(InventoryMessages.InventoryFull))
            {
                Debug.Log("Inventory is full...".Info());
            }

            var abstractItem = itemTable.GetAbstractItem();

            if (!inserted.Equals(InventoryMessages.Inserted) && abstractItem != null)
            {
                Destroy(abstractItem.gameObject);
            }
        }
    }
}