using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    
    private void Start() {
        playButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
            // GameManager.Instance.StartGame();
        });
        quitButton.onClick.AddListener(Application.Quit);
        
        // Unpause the game if it was paused
        Time.timeScale = 1f;
    }
}