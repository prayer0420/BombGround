using System;
using System.Collections.Generic;
using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Displays.PrefabFiller;
using Inventory.Scripts.Inventory.Enums;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Displays;
using Inventory.Scripts.ScriptableObjects.Items;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Inventory.Displays
{
    [RequireComponent(typeof(RectTransform))]
    public class AbstractContainerDisplay<T> : MonoBehaviour
    {
        [Header("Inventory Settings Anchor Settings")] [SerializeField]
        private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Container Sort")]
        [SerializeField]
        [Tooltip("This will be used to Sort the Containers in PLayer Inventory. Ps: This will be reverse in runtime.")]
        private ItemDataTypeSo[] itemDataTypeSos;

        [Header("Listening on...")] [SerializeField]
        private OnContainerInteractEventChannelSo<T> onContainerInteractEventChannelSo;

        private readonly List<T> _displayedContainers = new();

        private readonly List<DisplayFiller> _displayFillers = new();

        protected InventorySettingsAnchorSo InventorySettingsAnchorSo => inventorySettingsAnchorSo;

        protected RectTransform ContainerParent { get; private set; }

        private bool _initialRefreshUi;

        private void Awake()
        {
            if (inventorySettingsAnchorSo == null)
            {
                Debug.LogError("ContainerDisplay not configured correctly...".Configuration());
                return;
            }

            var displayFiller = inventorySettingsAnchorSo.InventorySettingsSo.DisplayFiller;

            if (displayFiller == null)
            {
                Debug.LogError("Display filler prefab not configured in InventorySettingsSos...".Settings());
            }

            ContainerParent = GetComponent<RectTransform>();

            Array.Reverse(itemDataTypeSos);
        }

        private void OnEnable()
        {
            if (onContainerInteractEventChannelSo == null)
            {
                Debug.LogError("Container Interact not configured in the Game Object...".Configuration());
                return;
            }

            onContainerInteractEventChannelSo.OnEventRaised += HandleInteraction;
        }

        private void OnDisable()
        {
            if (onContainerInteractEventChannelSo == null) return;

            onContainerInteractEventChannelSo.OnEventRaised -= HandleInteraction;
        }

        private void HandleInteraction(T container, InventoryInteractionStatus inventoryInteractionStatus)
        {
            if (_displayedContainers.Contains(container) &&
                inventoryInteractionStatus == InventoryInteractionStatus.Close)
            {
                _displayedContainers.Remove(container);
                OnRemoveDisplayContainer(container);
                ResortContainers();
                return;
            }

            if (_displayedContainers.Contains(container) ||
                inventoryInteractionStatus != InventoryInteractionStatus.Open) return;

            _displayedContainers.Add(container);
            OnAddDisplayContainer(container);
            ResortContainers();
            
            if (!_initialRefreshUi)
            {
                RefreshUI();
                _initialRefreshUi = true;
            }
        }
        
        private void RefreshUI()
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(ContainerParent);
        }

        protected DisplayFiller TryGetPrefab(ItemTable itemTable)
        {
            if (itemTable == null) return null;

            var findDisplayFillerBy = FindDisplayFillerBy(null);

            if (findDisplayFillerBy != null)
            {
                findDisplayFillerBy.Set(itemTable);

                return findDisplayFillerBy;
            }

            var displayFiller = inventorySettingsAnchorSo.InventorySettingsSo.DisplayFiller;

            var instantiate = Instantiate(displayFiller, ContainerParent);

            instantiate.gameObject.SetActive(false);
            _displayFillers.Add(instantiate);
            instantiate.Set(itemTable);

            return instantiate;
        }

        protected void RemoveDisplayFiller(ItemTable itemTable)
        {
            var displayFiller = FindDisplayFillerBy(itemTable);

            if (displayFiller == null) return;

            displayFiller.gameObject.SetActive(false);
            displayFiller.UnSet();
        }

        protected DisplayFiller FindDisplayFillerBy(ItemTable itemTable)
        {
            var found = _displayFillers.Find(filler => filler.CurrentInventoryItem == itemTable);

            return found ? found : null;
        }

        protected List<T> GetDisplayedContainers()
        {
            return _displayedContainers;
        }

        protected virtual void OnAddDisplayContainer(T container)
        {
        }

        protected virtual void OnRemoveDisplayContainer(T container)
        {
        }

        private void ResortContainers()
        {
            var allInventoryItems = GetAllInventoryItems();

            foreach (var itemDataTypeSo in itemDataTypeSos)
            {
                var inventoryItem = allInventoryItems
                    .Find(item => item.ItemDataSo.ItemDataTypeSo == itemDataTypeSo);

                if (inventoryItem == null) continue;

                var findDisplayFillerBy = FindDisplayFillerBy(inventoryItem);

                if (findDisplayFillerBy != null)
                {
                    findDisplayFillerBy.Sort();
                }
            }
        }

        protected virtual List<ItemTable> GetAllInventoryItems()
        {
            return new List<ItemTable>();
        }
    }
}