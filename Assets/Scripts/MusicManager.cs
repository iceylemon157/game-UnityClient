using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.UI;

public class MusicManager : MonoBehaviour {
    public static MusicManager Instance { get; private set; }
    [SerializeField] private AudioSource audioSource;
    private const string MusicPauseKey = "MusicPauseKey";
    
    private void Awake() {
        Instance = this;
    }
    
    private void Start() {
        var pause = PlayerPrefs.GetInt(MusicPauseKey, 0) == 1;
        Debug.Log("pause: " + pause);
        if (pause) {
            AudioListener.pause = true;
            audioSource.Pause();
        } else {
            AudioListener.pause = false;
            audioSource.Play();
        }
    }
    
    public int Toggle() {
        var pause = PlayerPrefs.GetInt(MusicPauseKey, 0);
        pause = 1 - pause;
        PlayerPrefs.SetInt(MusicPauseKey, pause);
        if (pause == 1) {
            AudioListener.pause = true;
            audioSource.Pause();
        } else {
            AudioListener.pause = false;
            audioSource.Play();
        }

        return pause;
    }
    
    public int GetMusicPause() {
        return PlayerPrefs.GetInt(MusicPauseKey, 0);
    }
}