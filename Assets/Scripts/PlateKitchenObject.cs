using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class PlateKitchenObject : KitchenObject {
    public event EventHandler<IngredientAddedEventArgs> OnIngredientAdded;

    public class IngredientAddedEventArgs : EventArgs {
        public KitchenObjectSO KitchenObjectSO;
    }

    [SerializeField] private List<KitchenObjectSO> validKitchenObjectSOList;

    private List<KitchenObjectSO> _kitchenObjectSOList;

    private void Awake() {
        _kitchenObjectSOList = new List<KitchenObjectSO>();
    }

    public bool TryAddIngredient(KitchenObjectSO kitchenObjectSO) {
        if (!validKitchenObjectSOList.Contains(kitchenObjectSO)) return false;
        if (_kitchenObjectSOList.Contains(kitchenObjectSO)) return false;
        _kitchenObjectSOList.Add(kitchenObjectSO);
        OnIngredientAdded?.Invoke(this, new IngredientAddedEventArgs() {
            KitchenObjectSO = kitchenObjectSO
        });

        return true;
    }

    public List<KitchenObjectSO> GetKitchenObjectSOList() {
        return _kitchenObjectSOList;
    }
}