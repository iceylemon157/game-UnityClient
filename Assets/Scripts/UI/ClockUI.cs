using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;
using UnityEngine.UI;

public class ClockUI : MonoBehaviour {
    [SerializeField] private Image timerImage;

    private void Start() {
        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
        timerImage.fillAmount = 0f;
        Hide();
    }

    private void Update() {
        if (GameManager.Instance.IsGamePlaying()) {
            timerImage.fillAmount = GameManager.Instance.GetGamePlayingRoundNormalized();
        }
    }

    private void GameManager_OnGameStateChanged(object sender, System.EventArgs e) {
        if (GameManager.Instance.IsGamePlaying()) {
            Show();
        } else {
            Hide();
        }
    }


    private void Show() {
        gameObject.SetActive(true);
    }

    private void Hide() {
        gameObject.SetActive(false);
    }
}