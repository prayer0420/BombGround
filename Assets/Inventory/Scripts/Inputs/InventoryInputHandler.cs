using UnityEngine;

namespace Inventory.Scripts.Inputs
{
    public class InventoryInputHandler : MonoBehaviour
    {
        [Header("Input Provider")] [SerializeField]
        private InputProviderSo inputProviderSo;

        private void Update()
        {
            inputProviderSo.Process();
        }
    }
}