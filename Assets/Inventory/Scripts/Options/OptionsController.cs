using System.Collections.Generic;
using Inventory.Scripts.Inventory.Items;
using Inventory.Scripts.ScriptableObjects.Events.Options;
using Inventory.Scripts.ScriptableObjects.Options;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.Scripts.Options
{
    [RequireComponent(typeof(RectTransform))]
    public class OptionsController : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private int maxOptions = 25;
        [SerializeField] private Button buttonPrefab;

        [Header("Broadcasting on...")] [SerializeField]
        private OnOptionInteractEventChannelSo onOptionInteractEventChannelSo;

        private List<OptionSo> _options;

        private bool _buttonsCreated;
        private Button[] _buttons;

        private void Start()
        {
            InitButtons();
        }

        private void InitButtons()
        {
            if (_buttonsCreated) return;

            _buttons = new Button[maxOptions];

            for (var i = 0; i < maxOptions; i++)
            {
                var buttonInstantiate = Instantiate(buttonPrefab, transform);

                _buttons[i] = buttonInstantiate.GetComponent<Button>();

                _buttons[i].onClick.RemoveAllListeners();

                buttonInstantiate.gameObject.SetActive(false);
            }

            _buttonsCreated = true;
        }

        public void InitOptions(AbstractItem inventoryItem, List<OptionSo> options)
        {
            if (options == null) return;

            _options = options;

            if (_buttons == null) InitButtons();

            DisableButtons();

            for (var i = 0; i < _options.Count; i++)
            {
                if (i >= 25) continue;

                var option = _options[i];

                if (option == null) return;

                var button = _buttons[i];

                var tmpText = button.GetComponentInChildren<TMP_Text>();

                tmpText.text = option.DisplayName;
                button.onClick.AddListener(() =>
                {
                    option.OnItemExecuteOptionEventChannelSo.RaiseEvent(inventoryItem);
                    gameObject.SetActive(false);
                });

                button.gameObject.SetActive(true);
            }
        }

        private void DisableButtons()
        {
            foreach (var button in _buttons)
            {
                button.onClick.RemoveAllListeners();
                button.gameObject.SetActive(false);
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            onOptionInteractEventChannelSo.RaiseEvent(this);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            onOptionInteractEventChannelSo.RaiseEvent(null);
        }
    }
}