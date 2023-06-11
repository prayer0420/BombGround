using Inventory.Editor.Utils;
using Inventory.Scripts.Helper;
using Inventory.Scripts.Inventory.Grids;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Editor
{
    public class UgiGameObjectCreationEditor : EditorWindow
    {
        private enum PriorityOrder
        {
            CreateGrid = 1000,
        }

        private static readonly Vector2 ItemGridElementSize = new(100f, 100f);

        [MenuItem("GameObject/UGI - UI/Item Grid", false, (int)PriorityOrder.CreateGrid)]
        private static void AddItemGrid(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UgiEditorUtils.FactorySwapToEditor())
                go = CreateItemGrid();

            UgiEditorUtils.PlaceUIElementRoot(go, menuCommand);
        }

        private static GameObject CreateItemGrid()
        {
            var go = UgiEditorUtils.CreateUIElementRoot("Item Grid", ItemGridElementSize, typeof(Image),
                typeof(ItemGrid2D),
                typeof(GridInteract2D));

            var itemGrid = go.GetComponent<ItemGrid2D>();
            var image = go.GetComponent<Image>();
            SetAnchorsForGrid(image.rectTransform);

            if (itemGrid.InventorySettingsAnchorSo != null)
            {
                image.sprite = itemGrid.InventorySettingsAnchorSo.InventorySettingsSo.InventorySlotSprite;
                itemGrid.ResizeGrid();
            }

            image.type = Image.Type.Tiled;

            return go;
        }

        private static void SetAnchorsForGrid(RectTransform image)
        {
            if (image == null)
            {
                Debug.LogError("Image is null... Could not update the anchors".Configuration());
                return;
            }

            var imageAnchoredPosition = new Vector2(0, 1);

            image.anchoredPosition = imageAnchoredPosition;
            image.anchorMin = imageAnchoredPosition;
            image.anchorMax = imageAnchoredPosition;
            image.pivot = imageAnchoredPosition;
        }
        
        [MenuItem("GameObject/UGI - UI/Container Item Grid", false, (int)PriorityOrder.CreateGrid)]
        private static void AddContainerItemGrid(MenuCommand menuCommand)
        {
            GameObject go;
            using (new UgiEditorUtils.FactorySwapToEditor())
                go = CreateContainerItemGrid();

            UgiEditorUtils.PlaceUIElementRoot(go, menuCommand);

            var rectTransform = go.GetComponent<RectTransform>();

            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTransform.rect.width);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rectTransform.rect.height);
        }

        private static GameObject CreateContainerItemGrid()
        {
            var go = UgiEditorUtils.CreateUIElementRoot("ContainerGrids_Container", ItemGridElementSize, typeof(RectTransform),
                typeof(ContainerGrids));
            
            var itemGrid = CreateItemGrid();

            var containerGridsRectTransform = go.GetComponent<RectTransform>();
            var rectTransform = itemGrid.GetComponent<RectTransform>();

            rectTransform.SetParent(containerGridsRectTransform);

            containerGridsRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTransform.rect.width);
            containerGridsRectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rectTransform.rect.height);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, rectTransform.rect.width);
            rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, rectTransform.rect.height);

            SetAnchorsForGrid(containerGridsRectTransform);
            
            return go;
        }
    }
}