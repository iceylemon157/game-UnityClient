using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSettingsUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button quitButton2;
    [SerializeField] private Dropdown recipeModeDropdown;
    
    
    private void Start() {
        playButton.onClick.AddListener(() => {
            Loader.Load(Loader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => {
            Debug.Log("Hi");
            var button2Image = quitButton2.GetComponent<Image>();
            button2Image.color = Color.magenta;
        });
        // quitButton.onClick.AddListener(Application.Quit);
        
        // Unpause the game if it was paused
        Time.timeScale = 1f;
    }
}