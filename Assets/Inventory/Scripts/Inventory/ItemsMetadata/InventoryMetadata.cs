using System;
using System.Collections.Generic;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects.Options;

namespace Inventory.Scripts.Inventory.ItemsMetadata
{
    [Serializable]
    public class InventoryMetadata
    {
        public List<OptionSo> OptionsMetadata { get; } = new();

        public ItemTable ItemTable { get; }

        public InventoryMetadata(ItemTable itemTable)
        {
            ItemTable = itemTable;
            InitializeOptions();
        }

        public virtual void OnPlaceItem(ItemTable item)
        {
        }

        private void InitializeOptions()
        {
            if (ItemTable == null) return;

            foreach (var optionSo in ItemTable.ItemDataSo.GetOptionsOrdered())
            {
                OptionsMetadata.Add(optionSo);
            }
        }
    }
}