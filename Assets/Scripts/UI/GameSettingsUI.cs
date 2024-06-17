using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;
using Image = UnityEngine.UI.Image;
using Toggle = UnityEngine.UI.Toggle;

public class GameSettingsUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button quitButton2;
    
    [SerializeField] private ToggleGroup recipeModeToggleGroup;
    private const string RecipeModeKey = "RecipeMode";
    
    private void Start() {
        playButton.onClick.AddListener(() => {
            PlayerPrefs.SetString(
                RecipeModeKey,
                recipeModeToggleGroup.ActiveToggles().First().GetComponent<RadioButtonUI>().recipeMode
                );
            Loader.Load(Loader.Scene.GameScene);
        });
        quitButton.onClick.AddListener(() => {
            // Debug.Log("Hi");
            var button2Image = quitButton2.GetComponent<Image>();
            button2Image.color = Color.magenta;
        });

        // quitButton.onClick.AddListener(Application.Quit);
        
        // Unpause the game if it was paused
        Time.timeScale = 1f;
    }
}