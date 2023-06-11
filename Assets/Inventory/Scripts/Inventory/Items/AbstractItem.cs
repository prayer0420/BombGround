using Inventory.Scripts.Inventory.Items.Enums;
using Inventory.Scripts.ScriptableObjects.Anchors;
using Inventory.Scripts.ScriptableObjects.Items;
using Inventory.Scripts.ScriptableObjects.Tile;
using UnityEngine;

namespace Inventory.Scripts.Inventory.Items
{
    public abstract class AbstractItem : MonoBehaviour
    {
        [SerializeField] private AbstractGridSelectedAnchorSo abstractGridSelectedAnchorSo;

        [SerializeField] protected TileSpecSo tileSpecSo;

        [SerializeField] protected Rotation rotationType = Rotation.MinusNinety;

        [field: SerializeReference] public ItemTable ItemTable { get; private set; }

        public void Set(ItemDataSo item, bool shouldInitMetadata = true)
        {
            ItemTable = new ItemTable(item, abstractGridSelectedAnchorSo);

            if (ItemTable.InventoryMetadata == null && shouldInitMetadata)
            {
                ItemTable.CreateMetadata();
            }

            ItemTable.OnUpdateUI += HandleUpdateUI;

            OnSetProperties(item);
        }

        private void OnDisable()
        {
            if (ItemTable == null) return;

            ItemTable.OnUpdateUI -= HandleUpdateUI;
        }

        private void HandleUpdateUI(ItemUIUpdater itemUIUpdater)
        {
            SetBackground(itemUIUpdater.Background.Item1, itemUIUpdater.Background.Item2);
        }

        protected abstract void OnSetProperties(ItemDataSo item);

        public abstract void ResizeIcon();

        public abstract void ResizeIconOnHolder(Vector2 holderSize);

        public void Rotate()
        {
            ItemTable.Rotate();

            RotateUI();
        }

        protected abstract void RotateUI();

        public void SetDragProps(ItemDataSo itemDataSo)
        {
            ItemTable.SetDragProps(itemDataSo);
            transform.SetAsFirstSibling();
        }

        public abstract void SetDragRepresentationDragStyle();

        public abstract void SetDragStyle();

        public abstract void UnsetDragStyle();

        // TODO: Validate in the future if Color is the best fits.
        public abstract void SetBackground(bool setBackground, Color? color = null);

        public void RefreshItem(ItemTable item)
        {
            ClearPreviousState(ItemTable);

            ItemTable = item;

            if (item.InventoryMetadata == null && !item.IsDragRepresentation)
            {
                ItemTable.CreateMetadata();
                ItemTable.InventoryMetadata.OnPlaceItem(ItemTable);
            }

            if (ItemTable.IsRotated)
            {
                RotateUI();
            }

            ItemTable.OnUpdateUI += HandleUpdateUI;

            OnSetProperties(ItemTable.ItemDataSo);
        }

        private void ClearPreviousState(ItemTable previousItemTable)
        {
            if (previousItemTable == null) return;

            previousItemTable.OnUpdateUI -= HandleUpdateUI;
        }
    }
}