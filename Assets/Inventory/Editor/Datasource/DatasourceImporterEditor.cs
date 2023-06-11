using System.Collections.Generic;
using System.Linq;
using Inventory.Scripts.Helper;
using Inventory.Scripts.ScriptableObjects.Items;
using Inventory.Scripts.ScriptableObjects.Items.Datasources;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Inventory.Editor.Datasource
{
    public class DatasourceImporterEditor : EditorWindow
    {
        [SerializeField] private DatasourceItems datasource;

        [SerializeField] private bool replaceAllItemsInDatasource = true;

        [SerializeField] private string pathToImportItems = "Inventory/Items/";

        [MenuItem("Ultimate Grid Inventory/Items DataSource Importer")]
        private static void Init()
        {
            // Get existing open window or if none, make a new one:
            var window = GetWindow<DatasourceImporterEditor>("Data Source Importer Settings");
            window.Show();
        }

        private void OnGUI()
        {
            EditorGUILayout.BeginVertical();
            GUILayout.Space(8);
            GUILayout.Label("Data Source Importer Settings", EditorStyles.boldLabel);


            GUILayout.Space(16);
            GUILayout.Label("Importer Settings", EditorStyles.boldLabel);

            datasource = EditorGUILayout.ObjectField("Datasource Items", datasource,
                typeof(DatasourceItems),
                true) as DatasourceItems;

            replaceAllItemsInDatasource =
                EditorGUILayout.Toggle("Replace all items in the Datasource", replaceAllItemsInDatasource);

            pathToImportItems = EditorGUILayout.TextField("Path to import items", pathToImportItems);

            GUILayout.Space(24);

            GUILayout.BeginHorizontal();

            EditorGUILayout.EndVertical();

            if (GUILayout.Button("Import Items"))
            {
                ImportItems();
                SaveScene();
            }

            GUILayout.EndHorizontal();
        }

        private void ImportItems()
        {
            Debug.Log($"Import items from path {pathToImportItems}".Editor());

            var allItemsInPath = GetAllItemsInPath(pathToImportItems);

            if (datasource == null)
            {
                Debug.Log("Datasource is null... Cannot import items...".Editor());
                return;
            }

            Debug.Log($"Importing {allItemsInPath.Count} items...".Editor());
            datasource.Import(allItemsInPath, !replaceAllItemsInDatasource);
            EditorUtility.SetDirty(datasource);
        }

        private List<ItemDataSo> GetAllItemsInPath(string path)
        {
            if (!Application.isEditor) return new List<ItemDataSo>();

#if UNITY_EDITOR
            var guids = AssetDatabase.FindAssets("t:ItemDataSo", new[] { ResolvePath(path) });

            return guids.Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<ItemDataSo>).ToList();
#endif
        }

        private static string ResolvePath(string path)
        {
            if (path == null)
            {
                Debug.LogError($"Path cannot be null... Please provide some value".Error());
                return null;
            }

            return path.StartsWith("Assets/") ? path : $"Assets/{path}";
        }

        private static void SaveScene()
        {
#if UNITY_EDITOR
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
#endif
        }
    }
}