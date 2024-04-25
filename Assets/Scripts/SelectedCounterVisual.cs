using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {
    [SerializeField] private GameObject[] visualGameObjectArray;

    [SerializeField] private BaseCounter baseCounter;
    private void Start() {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.SelectedCounterChangedEventArgs e) {
        if (e.SelectedCounter == baseCounter) {
            Show();
        } else {
            Hide();
        }
    }
    private void Show() {
        foreach (var visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(true);
        }
    }
    private void Hide() {
        foreach (var visualGameObject in visualGameObjectArray) {
            visualGameObject.SetActive(false);
        }
    }
}