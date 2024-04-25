using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterSound : MonoBehaviour {
    [SerializeField] private StoveCounter stoveCounter;
    private AudioSource _audioSource;

    private void Awake() {
        _audioSource = GetComponent<AudioSource>();
    }

    private void Start() {
        stoveCounter.OnFryingStateChange += StoveCounter_OnFryingStateChange;
    }

    private void StoveCounter_OnFryingStateChange(object sender, StoveCounter.FryingStateChangeEventArgs e) {
        if (e.FryingState is StoveCounter.FryingState.Frying or StoveCounter.FryingState.Fried) {
            _audioSource.Play();
        } else {
            _audioSource.Stop();
        }
    }
}