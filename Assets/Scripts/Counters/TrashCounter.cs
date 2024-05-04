using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TrashCounter : BaseCounter {
    public static event EventHandler OnAnyItemTrashed;
    
    public new static void ResetStaticData() {
        OnAnyItemTrashed = null;
    }

    public override void Interact(Player player) {
        if (!player.HasKitchenObject()) return;
        player.GetKitchenObject().DestroySelf();
        OnAnyItemTrashed?.Invoke(this, EventArgs.Empty);
    }
}