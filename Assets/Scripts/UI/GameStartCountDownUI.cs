using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;

public class GameStartCountDownUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI countDownText;

    private void Start() {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        Hide();
    }

    private void Update() {
        if (!GameManager.Instance.IsCountDown()) return;
        countDownText.text = Math.Ceiling(GameManager.Instance.GetCountDownTimer()).ToString(CultureInfo.InvariantCulture);
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e) {
        if (GameManager.Instance.IsCountDown()) {
            Show();
        } else {
            Hide();
        }
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}