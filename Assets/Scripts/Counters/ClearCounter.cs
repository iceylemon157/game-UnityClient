using UnityEngine;

public class ClearCounter : BaseCounter, IKitchenObjectParent {
    [SerializeField] private KitchenObjectSO kitchenObjectSO;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
            } else {
                if (player.GetKitchenObject().TryGetPlate(out var plate)) {
                    if (plate!.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                } else {
                    if (GetKitchenObject().TryGetPlate(out plate)) {
                        if (plate!.TryAddIngredient(player.GetKitchenObject().GetKitchenObjectSO())) {
                            player.GetKitchenObject().DestroySelf();
                        }
                    } else {
                        Debug.Log("You can't put a " + player.GetKitchenObject() + " on a " + GetKitchenObject() + "!");
                    }
                }
            }
        }
    }
}