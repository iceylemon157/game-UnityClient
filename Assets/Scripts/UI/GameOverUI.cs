using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour {
    [SerializeField] private TextMeshProUGUI recipeDeliveredText;
    [SerializeField] private TextMeshProUGUI finalScoreLabelText;
    [SerializeField] private Button restartButton;
    
    private const string ChosenTeamKey = "ChosenTeamKey";
    
    private void Start() {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        restartButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
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
        // Show the number of successful orders delivered
        // recipeDeliveredText.text = DeliveryManager.Instance.GetSuccessOrderDelivered().ToString();
        
        // Show the total score
        var teamName = $"Team {PlayerPrefs.GetInt(ChosenTeamKey, 1)}";
        finalScoreLabelText.text = teamName + " Score:";
        recipeDeliveredText.text = GameManager.Instance.GetTotalScore().ToString();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}