using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class AudioClipRefsSO : ScriptableObject {
    [SerializeField] public AudioClip[] chop;
    [SerializeField] public AudioClip[] deliveryFail;
    [SerializeField] public AudioClip[] deliverySuccess;
    [SerializeField] public AudioClip[] footstep;
    [SerializeField] public AudioClip[] objectDrop;
    [SerializeField] public AudioClip[] objectPickup;
    [SerializeField] public AudioClip[] panSizzle;
    [SerializeField] public AudioClip[] trash;
    [SerializeField] public AudioClip[] warning;
}