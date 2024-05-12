using UnityEngine;
using UnityEngine.UI;


namespace UI {
    public class SwitchToggleButton : MonoBehaviour {
        [SerializeField] private RectTransform uiHandle;
        [SerializeField] private RectTransform uiBackground;
        [SerializeField] private Color backgroundActiveColor;
        [SerializeField] private Color handleActiveColor;

        private Image _backgroundImage, _handleImage;
        private Color _backgroundDefaultColor, _handleDefaultColor;
        private Toggle _toggle;
        private Vector2 _handlePos;
        
        private const string IsServerModeKey = "IsServerMode";

        private void Awake() {
            _backgroundImage = uiBackground.GetComponent<Image>();
            _handleImage = uiHandle.GetComponent<Image>();
            _backgroundDefaultColor = _backgroundImage.color;
            _handleDefaultColor = _handleImage.color;

            _handlePos = uiHandle.anchoredPosition;
        }

        private void Start() {
            _toggle = GetComponent<Toggle>();
            _toggle.onValueChanged.AddListener(Toggle);

            if (_toggle.isOn) {
                // Maybe it is default on
                Toggle(true);
            }
        }

        private void Toggle(bool on) {
            uiHandle.anchoredPosition = on ? _handlePos * -1 : _handlePos;
            _backgroundImage.color = on ? backgroundActiveColor : _backgroundDefaultColor;
            _handleImage.color = on ? handleActiveColor : _handleDefaultColor;
            PlayerPrefs.SetInt(IsServerModeKey, on ? 1 : 0);
        }

    }
}