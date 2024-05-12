using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Transform gameSettingsUI;
    
    private void Start() {
        playButton.onClick.AddListener(() => {
            // Show the game settings UI
            gameSettingsUI.gameObject.SetActive(true);
        });
        quitButton.onClick.AddListener(Application.Quit);
        
        gameSettingsUI.gameObject.SetActive(false);
        
        // Unpause the game if it was paused
        Time.timeScale = 1f;
    }
}