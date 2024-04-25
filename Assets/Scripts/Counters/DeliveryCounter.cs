using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class DeliveryCounter : BaseCounter {
    public static DeliveryCounter Instance { get; private set; }
    private void Awake() {
        Instance = this;
    }
    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) return;
        if (player.GetKitchenObject().TryGetPlate(out var plate)) {
            player.GetKitchenObject().DestroySelf();
            var succeed = DeliveryManager.Instance.DeliverRecipe(plate);
            if (!succeed) {
                Debug.Log("You fucked up the delivery!");
            } else {
                Debug.Log("Tasty");
            }
        }
    }
}