using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerSounds : MonoBehaviour {
    private Player _player;
    public static event EventHandler OnWalking;

    private const float FootstepTimerMax = .2f;
    private float _footstepTimer;

    private void Awake() {
        _player = GetComponent<Player>();
    }

    private void Update() {
        _footstepTimer -= Time.deltaTime;
        if (_footstepTimer > 0f) return;

        _footstepTimer = FootstepTimerMax;
        if (_player.IsWalking()) {
            OnWalking?.Invoke(this, EventArgs.Empty);
        }
    }
}