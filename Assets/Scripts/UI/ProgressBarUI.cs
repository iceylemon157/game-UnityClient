using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUi : MonoBehaviour {
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgressGameObject;
    private IHasProgress _hasProgress;
    private void Start() {
        _hasProgress = hasProgressGameObject.GetComponent<IHasProgress>();
        if (_hasProgress == null) {
            Debug.LogError("Error: " + hasProgressGameObject + "does not have a component that implements IHasProgress");
            
        }
        _hasProgress!.OnProgressChanged += HasProgress_OnProgressChanged;
        barImage.fillAmount = 0;
        Hide();
    }

    private void HasProgress_OnProgressChanged(object sender, IHasProgress.ProgressChangedEventArgs e) {
        barImage.fillAmount = e.ProgressNormalized;
        if (e.ProgressNormalized is >= 1 or <= 0) {
            Hide();
        } else {
            Show();
        }
    }
    private void Show() {
        gameObject.SetActive(true);
    }
    private void Hide() {
        gameObject.SetActive(false);
    }
}