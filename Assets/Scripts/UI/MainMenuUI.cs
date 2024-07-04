using TMPro;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour {
    [SerializeField] private Button playButton;
    [SerializeField] private Button muteButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Transform gameSettingsUI;
    [SerializeField] private Transform duckVisual;
    [SerializeField] private TMP_Dropdown teamDropdown;
    
    private const string MusicPauseKey = "MusicPauseKey";
    private const string ChosenTeamKey = "ChosenTeamKey";
    
    private void Start() {
        playButton.onClick.AddListener(() => {
            // Check whether the team is selected
            if (teamDropdown.captionText.text == "Please select a team") {
                // Alert the player to select a team
                // Change the text of the dropdown to alert the player
                teamDropdown.captionText.text = "Please select a team first";
                return;
            }
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

        
        // Read the 12 teams from a file
        // read file, each team has it own folder
        // each folder has a file called team.txt
        // each team.txt has the team id, team name
        
        teamDropdown.options.Clear();
        
        // Ignore this, webgl doesn't have direct access to file system
        // var path = Application.dataPath + "/Teams";
        // var teamFolders = Directory.GetDirectories(path);
        var teamFolders = new string[16];
        var teamList = new List<string>(new string[teamFolders.Length]);
        
        for (var i = 1; i <= 16; i ++) {
            teamList[i - 1] = "Team " + i;
        }
        
        foreach (var folder in teamFolders) {
            
            // var teamFile = folder + "/team.txt";
            // if (!File.Exists(teamFile)) {
            //     Debug.LogError("team.txt not found in " + folder);
            //     continue;
            // }
            // var lines = File.ReadAllLines(teamFile);
            // if (lines.Length != 2) {
            //     Debug.LogError("team.txt should have 2 lines, found " + lines.Length + " in " + folder);
            //     continue;
            // }
            // var teamId = int.Parse(lines[0]);
            // var teamName = lines[1];
            // teamList[teamId - 1] = teamName;
        }
        
        teamDropdown.AddOptions(teamList);
        
        teamDropdown.onValueChanged.AddListener(value => {
            PlayerPrefs.SetInt(ChosenTeamKey, value + 1);
        });
        teamDropdown.captionText.text = "Please select a team";
        
        // for (var i = 1; i <= 12; i ++) {
        //     teamDropdown.options.Add(new TMP_Dropdown.OptionData("Team " + i));
        // }
        
        var pause = PlayerPrefs.GetInt(MusicPauseKey, 0) == 1;
        var tmPro = muteButton.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        tmPro.text = pause ? "Unmute" : "Mute";
        
        gameSettingsUI.gameObject.SetActive(false);
        
        // Unpause the game if it was paused
        Time.timeScale = 1f;
    }

}