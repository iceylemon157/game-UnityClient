using UnityEngine;

public interface IKitchenObjectParent {
    Transform GetKitchenObjectFollowTransform();
    KitchenObject GetKitchenObject();
    void SetKitchenObject(KitchenObject kitchenObject);
    void ClearKitchenObject();
    bool HasKitchenObject();
}