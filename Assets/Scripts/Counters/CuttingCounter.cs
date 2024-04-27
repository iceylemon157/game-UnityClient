using System;
using System.Linq;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {
    public static event EventHandler OnAnyCutting;

    public event EventHandler OnCutting;
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private CuttingRecipeSO[] cuttingRecipes;

    private int _cuttingProgress;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject() && HasRecipeFromInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _cuttingProgress = 0;
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                _cuttingProgress = 0;
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                    ProgressNormalized = 0
                });
            } else {
                if (player.GetKitchenObject() is PlateKitchenObject plate) {
                    if (plate!.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                } else {
                    Debug.Log("You can't put a " + player.GetKitchenObject() + " on a " + GetKitchenObject() + "!");
                }
            }
        }
    }

    public override void InteractAlternative(Player player) {
        if (!HasKitchenObject()) return;

        var kitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
        if (!HasRecipeFromInput(kitchenObjectSO)) return;
        _cuttingProgress ++;
        OnCutting?.Invoke(this, EventArgs.Empty);
        OnAnyCutting?.Invoke(this, EventArgs.Empty);
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
            ProgressNormalized = 1f * _cuttingProgress / GetCuttingRecipeSOFromInput(kitchenObjectSO).cuttingTime
        });
        if (_cuttingProgress >= GetCuttingRecipeSOFromInput(kitchenObjectSO).cuttingTime) {
            var output = GetOutputFromInput();
            GetKitchenObject().DestroySelf();
            KitchenObject.SpawnKitchenObject(output, this);
        }
    }

    private bool HasRecipeFromInput(KitchenObjectSO kitchenObjectSO) {
        return cuttingRecipes.Any(recipe => recipe.input == kitchenObjectSO);
    }

    private KitchenObjectSO GetOutputFromInput() {
        var kitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();

        // Shouldn't be null
        return cuttingRecipes.First(recipe => recipe.input == kitchenObjectSO).output;
    }

    private CuttingRecipeSO GetCuttingRecipeSOFromInput(KitchenObjectSO kitchenObjectSO) {
        return cuttingRecipes.FirstOrDefault(recipe => recipe.input == kitchenObjectSO);
    }
}