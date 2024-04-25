using UnityEngine;

[CreateAssetMenu()]
public class FryingRecipeSO : ScriptableObject {
    [SerializeField] public KitchenObjectSO input;
    [SerializeField] public KitchenObjectSO output;
    [SerializeField] public float fryingTime;
}