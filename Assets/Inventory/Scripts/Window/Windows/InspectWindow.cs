using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.Scripts.Window.Windows
{
    public class InspectWindow : Window
    {
        [Header("Inspect Window Settings")] [SerializeField]
        private Image itemIconImage;

        [SerializeField] private TMP_Text itemName;

        [SerializeField] private TMP_Text itemDescription;

        protected override void OnSetWindowProps()
        {
            SetTitle("Inspecting " + CurrentItemTable.ItemDataSo.DisplayName);

            itemIconImage.sprite = CurrentItemTable.ItemDataSo.Icon;
            itemName.text = CurrentItemTable.ItemDataSo.DisplayName;
        }
    }
}