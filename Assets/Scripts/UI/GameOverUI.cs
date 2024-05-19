using System.Collections;
using System.Collections.Generic;
using System;
using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeDeliveredText;
    private void Start() {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        Hide();
    }

    private void GameManager_OnGameStateChanged(object sender, EventArgs e) {
        if (GameManager.Instance.IsGameOver()) {
            SetRecipeDeliveredText();
            Show();
        } else {
            Hide();
        }
    }
    
    private void SetRecipeDeliveredText() {
        recipeDeliveredText.text = DeliveryManager.Instance.GetSuccessOrderDelivered().ToString();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}