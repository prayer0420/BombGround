using Inventory.Scripts.Inventory.Holders;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Inventory.Utils
{
    public class ItemHolderPreImage : MonoBehaviour
    {
        [SerializeField] private ItemHolder itemHolder;

        private Image _image;

        private void Start()
        {
            _image = GetComponent<Image>();
        }
    }
}
