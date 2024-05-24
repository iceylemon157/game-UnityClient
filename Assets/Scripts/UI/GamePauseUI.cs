using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePauseUI : MonoBehaviour {
    [SerializeField] private Button resumeButton;
    [SerializeField] private Button restartButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button muteButton;

    private void Start() {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnPaused += GameManager_OnGameUnPaused;
        
        resumeButton.onClick.AddListener(() => { GameManager.Instance.TogglePause(); });
        restartButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.GameScene); });
        mainMenuButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.MainMenuScene); });
        muteButton.onClick.AddListener(() => {
            AudioListener.pause = !AudioListener.pause;
            var tmPro = muteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            tmPro.text = AudioListener.pause ? "Unmute" : "Mute";
        });
        
        var tmPro = muteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        tmPro.text = AudioListener.pause ? "Unmute" : "Mute";
        
        Hide();
    }

    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }

    private void GameManager_OnGameUnPaused(object sender, System.EventArgs e) {
        Hide();
    }

    private void GameManager_OnGamePaused(object sender, System.EventArgs e) {
        Show();
    }
}