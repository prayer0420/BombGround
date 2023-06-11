using Inventory.Scripts.Options;
using UnityEngine;

namespace Inventory.Scripts.ScriptableObjects.Options.Configurations
{
    [CreateAssetMenu(menuName = "Inventory/Prefabs/New Options Config Prefab")]
    public class OptionsConfigurationSo : ScriptableObject
    {
        [SerializeField] private OptionsController optionsControllerPrefab;

        public OptionsController OptionsControllerPrefab => optionsControllerPrefab;
    }
}