using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounterVisual : MonoBehaviour {
    [SerializeField] private StoveCounter stoveCounter;
    [SerializeField] private GameObject stoveOnGameObject;
    [SerializeField] private GameObject particleGameObject;
    
    private bool _isFrying;

    private void Start() {
        stoveCounter.OnFryingStateChange += StoveCounter_OnFryingStateChange;
    }

    private void StoveCounter_OnFryingStateChange(object sender, StoveCounter.FryingStateChangeEventArgs e) {
        _isFrying = e.FryingState is StoveCounter.FryingState.Frying or StoveCounter.FryingState.Fried;
        stoveOnGameObject.SetActive(_isFrying);
        particleGameObject.SetActive(_isFrying);
    }
}