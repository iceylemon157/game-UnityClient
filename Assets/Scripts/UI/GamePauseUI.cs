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
    private const string MusicPauseKey = "MusicPauseKey";

    private void Start() {
        GameManager.Instance.OnGamePaused += GameManager_OnGamePaused;
        GameManager.Instance.OnGameUnPaused += GameManager_OnGameUnPaused;
        
        resumeButton.onClick.AddListener(() => { GameManager.Instance.TogglePause(); });
        restartButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.GameScene); });
        mainMenuButton.onClick.AddListener(() => { Loader.Load(Loader.Scene.MainMenuScene); });
        muteButton.onClick.AddListener(() => {
            var pause = MusicManager.Instance.Toggle();
            var tmPro = muteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            tmPro.text = pause == 1? "Unmute" : "Mute";
        });
        
        var pause = PlayerPrefs.GetInt(MusicPauseKey, 0) == 1;
        var tmPro = muteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        tmPro.text = pause ? "Unmute" : "Mute";
        
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