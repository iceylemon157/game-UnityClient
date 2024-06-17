using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button muteButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Transform gameSettingsUI;
    [SerializeField] private Transform duckVisual;
    
    private const string MusicPauseKey = "MusicPauseKey";
    
    private void Start() {
        playButton.onClick.AddListener(() => {
            // Show the game settings UI
            gameSettingsUI.gameObject.SetActive(true);
            duckVisual.gameObject.SetActive(false);
        });
        quitButton.onClick.AddListener(Application.Quit);
        muteButton.onClick.AddListener(() => {
            var pause = PlayerPrefs.GetInt(MusicPauseKey, 0);
            pause = 1 - pause;
            PlayerPrefs.SetInt(MusicPauseKey, pause);
            var tmPro = muteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
            tmPro.text = pause == 1? "Unmute" : "Mute";
        });
        
        var pause = PlayerPrefs.GetInt(MusicPauseKey, 0) == 1;
        var tmPro = muteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        tmPro.text = pause ? "Unmute" : "Mute";
        
        gameSettingsUI.gameObject.SetActive(false);
        
        // Unpause the game if it was paused
        Time.timeScale = 1f;
    }

}