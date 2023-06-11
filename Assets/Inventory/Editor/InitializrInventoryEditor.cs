using Inventory.Scripts;
using Inventory.Scripts.Controllers;
using Inventory.Scripts.ScriptableObjects;
using Inventory.Scripts.ScriptableObjects.Anchors;
using Inventory.Scripts.Window;
using Inventory.Scripts.Window.Windows;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inventory.Editor
{
    public class InitializrInventoryEditor : EditorWindow
    {
        [SerializeField] private GameObject prefabInventoryGrid;

        [SerializeField] private CanvasAnchorSo canvasAnchorSo;

        [SerializeField] private InventorySettingsAnchorSo inventorySettingsAnchorSo;

        // GameObjects Dependencies
        [SerializeField] private RectTransform defaultItemParentWhenDrag;
        [SerializeField] private RectTransform defaultOptionsParent;
        [SerializeField] private RectTransform windowParent;
        [SerializeField] private HighlighterAnchorSo highlighterAnchorSo;

        private Canvas _canvas;

        [MenuItem("Ultimate Grid Inventory/Initializr Inventory")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<InitializrInventoryEditor>(nameof(InitializrInventoryEditor));
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.inspectorDefaultMargins);
            GUILayout.Space(8);
            GUILayout.Label("Ultimate Grid Inventory Settings", EditorStyles.boldLabel);


            GUILayout.Space(16);
            GUILayout.Label("Initialization Settings", EditorStyles.boldLabel);

            prefabInventoryGrid = EditorGUILayout.ObjectField("Inventory Manager Prefab", prefabInventoryGrid,
                typeof(GameObject),
                true) as GameObject;

            inventorySettingsAnchorSo = EditorGUILayout.ObjectField("Inventory Settings Anchor So",
                inventorySettingsAnchorSo,
                typeof(InventorySettingsAnchorSo),
                true) as InventorySettingsAnchorSo;

            GUILayout.Space(8);
            GUILayout.Label("Inventory Manager Dependencies", EditorStyles.boldLabel);

            defaultItemParentWhenDrag = EditorGUILayout.ObjectField("Default Item Parent When Drag",
                defaultItemParentWhenDrag,
                typeof(RectTransform),
                true) as RectTransform;

            defaultOptionsParent = EditorGUILayout.ObjectField("Options Parent Settings", defaultOptionsParent,
                typeof(RectTransform),
                true) as RectTransform;

            windowParent = EditorGUILayout.ObjectField("Window Parent", windowParent,
                typeof(RectTransform),
                true) as RectTransform;

            highlighterAnchorSo = EditorGUILayout.ObjectField("HighlighterAnchorSo", highlighterAnchorSo,
                typeof(HighlighterAnchorSo),
                true) as HighlighterAnchorSo;

            GUILayout.Space(24);

            GUILayout.BeginHorizontal();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Initializr or Update"))
            {
                ApplyCanvasSettings();
                InstantiateRectTransforms();
                InitializrInventory();
                SaveScene();
            }

            GUILayout.EndHorizontal();
        }

        private void ApplyCanvasSettings()
        {
            _canvas = FindObjectOfType<Canvas>();

            if (_canvas == null)
            {
                Debug.LogError(
                    "Please create a canvas in the scene in order to complete the Ultimate Grid Inventory configuration.");
                return;
            }

            var canvasInitializer = GetCanvasInitializer(_canvas);

            canvasInitializer.SetAnchors(canvasAnchorSo);
        }

        private void InstantiateRectTransforms()
        {
            defaultItemParentWhenDrag = TryInstantiate(defaultItemParentWhenDrag);
            defaultOptionsParent = TryInstantiate(defaultOptionsParent);
            windowParent = TryInstantiate(windowParent);

            windowParent.SetAsLastSibling();
            defaultItemParentWhenDrag.SetAsLastSibling();
            defaultOptionsParent.SetAsLastSibling();
        }

        private RectTransform TryInstantiate(RectTransform rectTransform)
        {
            if (GameObject.Find(rectTransform.name)) return rectTransform;

            var gameObject = PrefabUtility.InstantiatePrefab(rectTransform, _canvas.transform) as RectTransform;

            if (gameObject != null)
            {
                gameObject.name = rectTransform.name;
            }

            return gameObject;
        }

        private void InitializrInventory()
        {
            var foundInventoryManagerInScene = FindObjectOfType<DraggableController>();

            if (foundInventoryManagerInScene != null)
            {
                Debug.LogWarning("Found Inventory Manager in Scene, altering properties...");
                AlterProperties(foundInventoryManagerInScene.gameObject);
                return;
            }

            Debug.Log("Instantiating Inventory Manager Prefab. Prefab Name: " + prefabInventoryGrid.name);
            var inventoryManager = PrefabUtility.InstantiatePrefab(prefabInventoryGrid) as GameObject;

            if (inventoryManager != null)
            {
                inventoryManager.transform.SetAsFirstSibling();
            }

            AlterProperties(inventoryManager);
        }

        private static CanvasInitializer GetCanvasInitializer(Canvas canvas)
        {
            var canvasInitializer = canvas.gameObject.GetComponent<CanvasInitializer>();

            return canvasInitializer != null ? canvasInitializer : canvas.gameObject.AddComponent<CanvasInitializer>();
        }

        private void AlterProperties(GameObject inventoryManager)
        {
            var draggableController = inventoryManager.GetComponent<DraggableController>();

            if (draggableController != null)
            {
                AlterDraggableController(draggableController);
            }

            var inventoryHighlight = inventoryManager.GetComponent<InventoryHighlightController>();

            if (inventoryHighlight != null)
            {
                AlterInventoryHighlight(inventoryHighlight);
            }

            var inventoryScrollController = inventoryManager.GetComponent<InventoryScrollController>();

            if (inventoryScrollController != null)
            {
                AlterScrollController(inventoryScrollController);
            }

            var optionsController = inventoryManager.GetComponent<InventoryOptionsController>();

            if (optionsController != null)
            {
                AlterOptionsController(optionsController);
            }

            var inventoryHighlightSpaceContainer =
                inventoryManager.GetComponent<InventoryHighlightSpaceContainerController>();

            if (inventoryHighlightSpaceContainer != null)
            {
                AlterInventoryHighlightSpaceContainer(inventoryHighlightSpaceContainer);
            }

            AlterComponentInChildren(inventoryManager);
        }

        private void AlterDraggableController(DraggableController draggableController)
        {
            EditorUtility.SetDirty(draggableController);
            draggableController.SetItemParent(defaultItemParentWhenDrag);
            draggableController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterInventoryHighlight(InventoryHighlightController inventoryHighlightController)
        {
            EditorUtility.SetDirty(inventoryHighlightController);
            inventoryHighlightController.SetHighlighterAnchorSo(highlighterAnchorSo);
            inventoryHighlightController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterScrollController(InventoryScrollController inventoryScrollController)
        {
            EditorUtility.SetDirty(inventoryScrollController);
            inventoryScrollController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterOptionsController(InventoryOptionsController optionsController)
        {
            EditorUtility.SetDirty(optionsController);
            optionsController.SetOptionsParent(defaultOptionsParent);
            optionsController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        private void AlterInventoryHighlightSpaceContainer(
            InventoryHighlightSpaceContainerController inventoryHighlightSpaceContainerController)
        {
            EditorUtility.SetDirty(inventoryHighlightSpaceContainerController);
            inventoryHighlightSpaceContainerController.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
        }

        // ========== Altering Children Components ==========

        private void AlterComponentInChildren(GameObject inventoryManager)
        {
            AlterWindowManager(inventoryManager);
        }

        private void AlterWindowManager(GameObject inventoryManager)
        {
            var inspectWindowManager = inventoryManager.GetComponentInChildren<InspectWindowManager>();

            if (inspectWindowManager != null)
            {
                AlterWindowManager(inspectWindowManager);
            }

            var containerWindowManager = inventoryManager.GetComponentInChildren<ContainerWindowManager>();

            if (containerWindowManager != null)
            {
                AlterWindowManager(containerWindowManager);
            }
        }

        private void AlterWindowManager(WindowManager windowManager)
        {
            EditorUtility.SetDirty(windowManager);
            windowManager.SetInventorySettingsAnchorSo(inventorySettingsAnchorSo);
            windowManager.SetWindowParent(windowParent);
        }

        private static void SaveScene()
        {
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
        }
    }
}