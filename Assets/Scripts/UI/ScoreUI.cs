using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreUI : MonoBehaviour {
    [SerializeField] private TMPro.TextMeshProUGUI scoreText;
    
    private void Start() {
        DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        scoreText.text = "0";
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e) {
        if (GameManager.Instance.IsGamePlaying()) {
            Show();
        } else {
            Hide();
        }
    }

    private void DeliveryManager_OnDeliverySuccess(object sender, EventArgs e) {
        scoreText.text = GameManager.Instance.GetTotalScore().ToString();
    }
    
    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}