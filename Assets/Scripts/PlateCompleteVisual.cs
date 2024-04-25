using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour {
    
    [Serializable]
    private struct Ingredient {
        public KitchenObjectSO kitchenObjectSO;
        public GameObject gameObject;
    }
    
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private List<Ingredient> ingredients; 
    
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObjectOnOnIngredientAdded;
        foreach (var ingredient in ingredients) {
            ingredient.gameObject.SetActive(false);
        }
    }

    private void PlateKitchenObjectOnOnIngredientAdded(object sender, PlateKitchenObject.IngredientAddedEventArgs e) {
        foreach (var ingredient in ingredients.Where(ingredient => ingredient.kitchenObjectSO == e.KitchenObjectSO)) {
            ingredient.gameObject.SetActive(true);
        }
    }
}