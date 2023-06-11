using System;
using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Grids;
using Inventory.Scripts.Inventory.Grids.Renderer;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.Options;
using Inventory.Scripts.ScriptableObjects.Items;
using Inventory.Scripts.ScriptableObjects.Options;
using UnityEngine;

namespace Inventory.Scripts.Inventory.ItemsMetadata
{
    [Serializable]
    public class ContainerMetadata : InventoryMetadata
    {
        private Dictionary<string, ContainerGrids> UIGridsOpened { get; } = new();

        private RendererGrids _rendererGrids;

        [field: SerializeReference] public List<GridTable> GridsInventory { get; private set; }

        public ContainerMetadata(ItemTable itemTable) : base(itemTable)
        {
            SetProps();
        }

        private void SetProps()
        {
            GridsInventory = new List<GridTable>();

            var containerGridsPrefab = GetPrefabGrids();
            var abstractGrids = containerGridsPrefab.GetAbstractGrids();

            foreach (var abstractGrid in abstractGrids)
            {
                GridsInventory.Add(new GridTable(abstractGrid.GridWidth, abstractGrid.GridHeight));
            }

            _rendererGrids = new RendererGrids2D();
        }

        public override void OnPlaceItem(ItemTable item)
        {
            base.OnPlaceItem(item);

            AddOption(OptionsType.OPEN);
            RemoveOption(OptionsType.UNEQUIP);
            AddOption(OptionsType.EQUIP);
        }

        public void OnEquipContainerHolder()
        {
            RemoveOption(OptionsType.EQUIP);
            AddOption(OptionsType.UNEQUIP);
        }

        public void OpenInventory(string key, Transform parentTransform)
        {
            if (parentTransform == null)
            {
                Debug.LogError("Opening Inventory. ParentTransform cannot be null...".Error());
                return;
            }

            var containerGridsPrefab = GetPrefabGrids();

            var containerGrids =
                _rendererGrids.Hydrate(parentTransform, containerGridsPrefab, GridsInventory);

            UIGridsOpened.TryAdd(key, containerGrids);
        }

        public void CloseInventory(string key)
        {
            if (UIGridsOpened.Count == 0) return;

            if (!UIGridsOpened.TryGetValue(key, out var containerGrids)) return;

            _rendererGrids.Dehydrate(containerGrids, GridsInventory);

            UIGridsOpened.Remove(key);
        }

        public bool IsInsertingInsideYourself(GridTable grid)
        {
            var gridTablesList = GridsInventory;

            if (gridTablesList.Count == 0) return false;

            if (gridTablesList.Contains(grid))
            {
                return true;
            }

            foreach (var gridTable in gridTablesList)
            {
                foreach (var containerItem in gridTable.GetAllContainersFromGrid())
                {
                    var containerItemInventoryMetadata = (ContainerMetadata)containerItem.InventoryMetadata;

                    var containsGrid = containerItemInventoryMetadata.GridsInventory
                        .Contains(grid);

                    if (containsGrid)
                    {
                        return true;
                    }

                    if (containerItemInventoryMetadata.IsInsertingInsideYourself(grid))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public InventoryMessages PlaceItemInInventory(ItemTable selectedInventoryItem)
        {
            var gridTables = GridsInventory;

            InventoryMessages? status = null;

            var firstRotation = selectedInventoryItem.IsRotated;

            foreach (var gridTable in gridTables)
            {
                var posInGrid = gridTable.FindSpaceForObjectAnyDirection(selectedInventoryItem);

                if (posInGrid == null) continue;

                if (gridTable == selectedInventoryItem.CurrentGridTable)
                {
                    status = InventoryMessages.AlreadyInserted;
                    break;
                }

                status = gridTable.PlaceItem(selectedInventoryItem, posInGrid.Value.x, posInGrid.Value.y);

                if (status != InventoryMessages.Inserted) continue;

                var abstractItem = selectedInventoryItem.GetAbstractItem();

                if (abstractItem != null)
                {
                    GameObject.Destroy(abstractItem.gameObject);
                }

                break;
            }

            if (status != InventoryMessages.Inserted && selectedInventoryItem.IsRotated != firstRotation)
            {
                selectedInventoryItem.Rotate();
            }

            return status ?? InventoryMessages.InventoryFull;
        }

        public bool ContainsSpaceForItem(AbstractItem selectedInventoryItem)
        {
            var lastRotateState = selectedInventoryItem.ItemTable.IsRotated;

            var gridTables = GridsInventory;

            var availablePosOnGrid = gridTables
                .Select(gridTable => gridTable.FindSpaceForObjectAnyDirection(selectedInventoryItem.ItemTable))
                .FirstOrDefault(posInGrid => posInGrid != null);

            if (lastRotateState != selectedInventoryItem.ItemTable.IsRotated)
            {
                selectedInventoryItem.Rotate();
            }

            return availablePosOnGrid.HasValue;
        }

        public Dictionary<string, ContainerGrids>.ValueCollection GetContainerGrids()
        {
            return UIGridsOpened.Values;
        }

        private void AddOption(OptionsType optionsType)
        {
            var optionSo = FindOption(optionsType);

            if (optionSo == null) return;

            AddToOptions(optionSo);
        }

        private void AddToOptions(OptionSo optionSo)
        {
            if (ItemTable.InventoryMetadata.OptionsMetadata.Contains(optionSo)) return;

            ItemTable.InventoryMetadata.OptionsMetadata.Add(optionSo);
        }

        private void RemoveOption(OptionsType optionsType)
        {
            var optionSo = FindOption(optionsType);

            if (optionSo == null) return;

            ItemTable.InventoryMetadata.OptionsMetadata.Remove(optionSo);
        }

        private OptionSo FindOption(OptionsType optionsType)
        {
            var itemDataSo = ItemTable.ItemDataSo;

            var options = itemDataSo.GetOptionsOrdered();

            var optionSo = Array.Find(options, so => so.OptionsType == optionsType);

            return optionSo;
        }

        private ContainerGrids GetPrefabGrids()
        {
            var itemContainerDataSo = (ItemContainerDataSo)ItemTable.ItemDataSo;

            return itemContainerDataSo.ContainerGrids;
        }
    }
}