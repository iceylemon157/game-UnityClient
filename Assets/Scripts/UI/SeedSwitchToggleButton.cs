using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    public class SeedSwitchToggleButton : MonoBehaviour {
        [SerializeField] private RectTransform uiHandle;
        [SerializeField] private RectTransform uiBackground;
        [SerializeField] private TMP_InputField inputField;
        [SerializeField] private Color backgroundActiveColor;
        [SerializeField] private Color handleActiveColor;

        private Image _backgroundImage, _handleImage;
        private Color _backgroundDefaultColor, _handleDefaultColor;
        private Toggle _toggle;
        private Vector2 _handlePos;

        private bool _isOn;
        private float _handleSize;

        private void Awake() {
            _backgroundImage = uiBackground.GetComponent<Image>();
            _handleImage = uiHandle.GetComponent<Image>();
            _backgroundDefaultColor = _backgroundImage.color;
            _handleDefaultColor = _handleImage.color;
            inputField.interactable = false;

            _handlePos = uiHandle.anchoredPosition;
            
        }

        private void Start() {
            
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(Toggle);
            // This button is not implemented yet, hence not interactable
            _toggle.interactable = false;

            if (_toggle.isOn) {
                // Maybe it is default on
                Toggle(true);
            }
        }

        private void Toggle(bool on) {
            uiHandle.anchoredPosition = on ? _handlePos * -1 : _handlePos;
            _backgroundImage.color = on ? backgroundActiveColor : _backgroundDefaultColor;
            _handleImage.color = on ? handleActiveColor : _handleDefaultColor;
            inputField.interactable = on;
        }
    }
}