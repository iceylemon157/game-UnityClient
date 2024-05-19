using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryManagerUI : MonoBehaviour {
    // Container is the parent of the waiting recipe UI elements
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;

    private void Awake() {
        recipeTemplate.gameObject.SetActive(false);
    }
    
    private void Start() {
        DeliveryManager.Instance.OnOrderSpawned += DeliveryManagerOnOrderSpawned;
        DeliveryManager.Instance.OnOrderCompleted += DeliveryManagerOnOrderCompleted;
        UpdateVisual();
    }

    private void DeliveryManagerOnOrderCompleted(object sender, DeliveryManager.OrderEventArgs e) {
        // Debug.Log("You should definitely update the visual now!");
        UpdateVisual();
    }

    private void DeliveryManagerOnOrderSpawned(object sender, EventArgs e) {
        UpdateVisual();
    }

    private void UpdateVisual() {
        foreach (Transform child in container) {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (var waitingOrder in DeliveryManager.Instance.GetWaitingOrders()) {
            var recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            recipeTransform.GetComponent<RecipeUI>().SetRecipeSO(waitingOrder.RecipeSO);
        }
        Debug.Log("Finishing Updating visual for waiting recipes");
    }
}