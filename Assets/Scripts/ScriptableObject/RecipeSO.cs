using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
[Serializable]
public class RecipeSO : ScriptableObject {
    private const int RecipeScoreMax = 1000;
    
    public int id;
    public int recipeScore = RecipeScoreMax;
    [SerializeField] public List<KitchenObjectSO> kitchenObjectSOList;
    [SerializeField] public string recipeName;
    
    [Serializable]
    public class RecipeData {
        public int id;
        public string recipeName;
        public int recipeScore;
    }
    
    public RecipeSO(RecipeSO recipeSO) {
        recipeScore = recipeSO.recipeScore;
        kitchenObjectSOList = recipeSO.kitchenObjectSOList;
        recipeName = recipeSO.recipeName;
    }
}