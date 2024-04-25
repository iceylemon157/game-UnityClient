using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour {
    [SerializeField] private PlateKitchenObject plateKitchenObject;
    [SerializeField] private Transform iconTemplate;
    
    private void Start() {
        plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
    }

    private void PlateKitchenObject_OnIngredientAdded(object sender, PlateKitchenObject.IngredientAddedEventArgs e) {
        foreach (var kitchenObjectSO in plateKitchenObject.GetKitchenObjectSOList()) {
            if (kitchenObjectSO == e.KitchenObjectSO) {
                var iconTransform = Instantiate(iconTemplate, transform);
                iconTransform.GetComponent<PlateIconSingleUI>().SetKitchenObjectSO(kitchenObjectSO);
                iconTransform.gameObject.SetActive(true);
            }
        }
    }
}