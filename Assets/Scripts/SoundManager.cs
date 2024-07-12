using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour {
    
    public SoundManager Instance { get; private set; }
    
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    
    private void Awake() {
        Instance = this;
    }

    private void Start() {
        DeliveryManager.Instance.OnDeliverySuccess += DeliveryManager_OnDeliverySuccess;
        DeliveryManager.Instance.OnDeliveryFailed += DeliveryManager_OnDeliveryFailed;
        CuttingCounter.OnAnyCutting += CuttingCounter_OnAnyCutting;
        Player.Instance.OnPickedUpSomething += Player_OnPickedUpSomething;
        BaseCounter.OnAnyItemPlaced += BaseCounter_OnAnyItemPlaced;
        TrashCounter.OnAnyItemTrashed += TrashCounter_OnAnyItemTrashed;
        PlayerSounds.OnWalking += PlayerSounds_OnWalking;
    }

    private void PlayerSounds_OnWalking(object sender, EventArgs e) { 
        PlayFootstepSound();
    }

    private void TrashCounter_OnAnyItemTrashed(object sender, EventArgs e) {
        PlaySound(audioClipRefsSO.trash, (sender as TrashCounter)!.transform.position);
    }

    private void BaseCounter_OnAnyItemPlaced(object sender, EventArgs e) {
        PlaySound(audioClipRefsSO.objectDrop, (sender as BaseCounter)!.transform.position);
    }

    private void Player_OnPickedUpSomething(object sender, EventArgs e) {
        PlaySound(audioClipRefsSO.objectPickup, Player.Instance.transform.position);
    }

    private void CuttingCounter_OnAnyCutting(object sender, EventArgs e) {
        var cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter!.transform.position);
    }

    private void DeliveryManager_OnDeliveryFailed(object sender, EventArgs e) {
        PlaySound(audioClipRefsSO.deliveryFail, DeliveryCounter.Instance.transform.position);
    }

    private void DeliveryManager_OnDeliverySuccess(object sender, EventArgs e) {
        PlaySound(audioClipRefsSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f) {
        var audioClip = audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)];
        PlaySound(audioClip, position, volume);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f) {
        AudioSource.PlayClipAtPoint(audioClip, position, volume);
    }
    
    private void PlayFootstepSound(float volume = .6f) {
        var audioClip = audioClipRefsSO.footstep[UnityEngine.Random.Range(0, audioClipRefsSO.footstep.Length)];
        PlaySound(audioClip, Player.Instance.transform.position, volume);
    }
}