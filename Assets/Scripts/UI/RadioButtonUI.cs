using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RadioButtonUI : MonoBehaviour {
    [SerializeField] private Image selectedButtonImage;
    
    private Toggle _toggle;
    
    private void Awake() {
        _toggle = GetComponent<Toggle>();
        _toggle.onValueChanged.AddListener(Toggle);
    }
    
    private void Toggle(bool on) {
        // selectedButtonImage.enabled = on;
        selectedButtonImage.gameObject.SetActive(on);
    }
}