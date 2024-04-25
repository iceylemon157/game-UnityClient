using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterAnimator : MonoBehaviour {
    private const string OpenClose = "OpenClose";
    [SerializeField] private ContainerCounter containerCounter;
    private Animator _animator;
    private void Awake() {
        _animator = GetComponent<Animator>();
    }
    private void Start() {
        containerCounter.OnPlayerGrabbedKitchenObject += ContainerCounter_OnPlayerGrabbedKitchenObject;
    }

    private void ContainerCounter_OnPlayerGrabbedKitchenObject(object sender, EventArgs e) {
        _animator.SetTrigger(OpenClose);
    }
}