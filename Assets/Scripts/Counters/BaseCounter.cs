using System;
using UnityEngine;

public class BaseCounter : MonoBehaviour, IKitchenObjectParent {
    public static event EventHandler OnAnyItemPlaced;

    [SerializeField] private Transform counterTop;

    private KitchenObject _kitchenObject;
    
    public static void ResetStaticData() {
        OnAnyItemPlaced = null;
    }

    public virtual void Interact(Player player) {
        // throw new NotImplementedException();
    }

    public virtual void InteractAlternative(Player player) {
        // do nothing if not overridden
        // throw new NotImplementedException();
    }

    public Transform GetKitchenObjectFollowTransform() {
        return counterTop;
    }

    public KitchenObject GetKitchenObject() {
        return _kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        _kitchenObject = kitchenObject;
        if (_kitchenObject != null) {
            OnAnyItemPlaced?.Invoke(this, EventArgs.Empty);
        }
    }

    public void ClearKitchenObject() {
        _kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return _kitchenObject != null;
    }
}