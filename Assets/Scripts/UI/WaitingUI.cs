using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class WaitingUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI waitingText;
    
    private bool _isShowing;

    private void Start() {
        _isShowing = true;
        Show();
    }

    private void Update() {
        if (GameManager.Instance.IsGameStarted()) {
            if (!_isShowing) return;
            Hide();
            _isShowing = false;
            return;
        }

        if (!_isShowing) {
            Show();
            _isShowing = true;
        }
        waitingText.text = "Waiting for teams...";
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}