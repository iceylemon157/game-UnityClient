using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu()]
[Serializable]
public class RecipeSO : ScriptableObject {
    [FormerlySerializedAs("ID")] public int id;
    public int recipeScore;
    [SerializeField] public List<KitchenObjectSO> kitchenObjectSOList;
    [SerializeField] public string recipeName;
}