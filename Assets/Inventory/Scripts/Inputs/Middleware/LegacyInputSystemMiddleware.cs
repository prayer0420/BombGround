﻿#if ENABLE_LEGACY_INPUT_MANAGER && !ENABLE_INPUT_SYSTEM
#define USE_LEGACY_INPUT_SYSTEM
using System;
#endif
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Events.Items;
using UnityEngine;

namespace Inventory.Scripts.Inputs.Middleware
{
    [CreateAssetMenu(menuName = "Inventory/Inputs/Legacy Input System Middleware")]
    public class LegacyInputSystemMiddleware : InputMiddleware
    {
        [SerializeField] private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        [Header("Event Draggable")] [SerializeField]
        private OnAbstractItemBeingDragEventChannelSo onAbstractItemBeingDragEventChannelSo;

#if USE_LEGACY_INPUT_SYSTEM
        public override event Action OnGenerateItem;
        public override event Action OnPickupItem;
        public override event Action OnReleaseItem;
        public override event Action OnToggleOptions;
        public override event Action OnRotateItem;
#endif

        private bool _isDragging;

        private void Awake()
        {
#if !ENABLE_LEGACY_INPUT_MANAGER
            Debug.Log("Legacy inputs are not enabled... Not handling them...");
#endif
        }

        private void OnEnable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised += ChangeIsDragging;
        }

        private void OnDisable()
        {
            onAbstractItemBeingDragEventChannelSo.OnEventRaised -= ChangeIsDragging;
        }

        private void ChangeIsDragging(AbstractItem inventoryItem)
        {
            _isDragging = inventoryItem != null;
        }

        public override void Process(InputState inputState)
        {
#if USE_LEGACY_INPUT_SYSTEM
            inputState.CursorPosition = Input.mousePosition;
            inputState.GridMovement = GetGridMovement();

            if (Input.GetKeyDown(KeyCode.W))
            {
                OnGenerateItem?.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.R))
            {
                OnRotateItem?.Invoke();
            }

            if (Input.GetMouseButtonDown(1))
            {
                OnToggleOptions?.Invoke();
            }

            if (ShouldHoldToDrag())
            {
                if (Input.GetMouseButtonUp(0))
                {
                    OnReleaseItem?.Invoke();
                }
            }
            else
            {
                if (_isDragging && Input.GetMouseButtonDown(0))
                {
                    OnReleaseItem?.Invoke();
                    return;
                }
            }

            if (Input.GetMouseButtonDown(0))
            {
                OnPickupItem?.Invoke();
            }
#endif
        }

        private Vector2 GetGridMovement()
        {
            var gridMovement = Vector2.zero;

#if USE_LEGACY_INPUT_SYSTEM
            if (Input.GetKey(KeyCode.UpArrow))
            {
                gridMovement.y += 1;
            }

            if (Input.GetKey(KeyCode.DownArrow))
            {
                gridMovement.y += -1;
            }

            if (Input.GetKey(KeyCode.RightArrow))
            {
                gridMovement.x += 1;
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                gridMovement.x += -1;
            }
#endif

            return gridMovement;
        }

#if USE_LEGACY_INPUT_SYSTEM
        private bool ShouldHoldToDrag()
        {
            return inventorySettingsAnchorSo.InventorySettingsSo.ShouldHoldToDrag;
        }
#endif
    }
}