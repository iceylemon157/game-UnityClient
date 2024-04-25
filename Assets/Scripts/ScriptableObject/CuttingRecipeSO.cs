using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject {
    [SerializeField] public KitchenObjectSO input;
    [SerializeField] public KitchenObjectSO output;
    [SerializeField] public int cuttingTime;
}