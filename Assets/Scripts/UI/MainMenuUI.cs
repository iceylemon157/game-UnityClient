using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button muteButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Transform gameSettingsUI;
    [SerializeField] private Transform duckVisual;
    
    private void Start() {
        playButton.onClick.AddListener(() => {
            // Show the game settings UI
            gameSettingsUI.gameObject.SetActive(true);
            duckVisual.gameObject.SetActive(false);
        });
        quitButton.onClick.AddListener(Application.Quit);
        muteButton.onClick.AddListener(() => {
            AudioListener.pause = !AudioListener.pause;
            var tmPro = muteButton.GetComponentInChildren<TextMeshProUGUI>();
            tmPro.text = AudioListener.pause ? "Unmute" : "Mute";
        });
        
        var tmPro = muteButton.GetComponentInChildren<TextMeshProUGUI>();
        tmPro.text = AudioListener.pause ? "Unmute" : "Mute";
        
        gameSettingsUI.gameObject.SetActive(false);
        
        // Unpause the game if it was paused
        Time.timeScale = 1f;
    }
}