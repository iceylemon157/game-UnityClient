using System;
using System.Linq;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {
    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;

    public enum FryingState {
        Idle,
        Frying,
        Fried,
        Burnt
    }

    public event EventHandler<OnFryingStateChangeEventArgs> OnFryingStateChange;

    public class OnFryingStateChangeEventArgs : EventArgs {
        public FryingState FryingState;
    }

    private float _fryingProgress;
    private FryingRecipeSO _fryingRecipeSO;
    private FryingState _fryingState;

    private void Start() {
        _fryingState = FryingState.Idle;
    }

    private void Update() {
        if (HasKitchenObject()) {
            switch (_fryingState) {
                case FryingState.Idle:
                    break;
                case FryingState.Frying:
                    _fryingProgress += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                        ProgressNormalized = 1f * _fryingProgress / _fryingRecipeSO.fryingTime
                    });
                    if (_fryingProgress >= _fryingRecipeSO.fryingTime) {
                        _fryingProgress = 0;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);

                        _fryingState = FryingState.Fried;
                        OnFryingStateChange?.Invoke(this, new OnFryingStateChangeEventArgs() {
                            FryingState = _fryingState
                        });
                        _fryingRecipeSO = GetFryingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                    }

                    break;
                case FryingState.Fried:
                    _fryingProgress += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                        ProgressNormalized = 1f * _fryingProgress / _fryingRecipeSO.fryingTime
                    });
                    if (_fryingProgress >= _fryingRecipeSO.fryingTime) {
                        _fryingProgress = 0;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);
                        _fryingState = FryingState.Burnt;
                        OnFryingStateChange?.Invoke(this, new OnFryingStateChangeEventArgs() {
                            FryingState = _fryingState
                        });
                    }

                    break;
                case FryingState.Burnt:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject() && HasRecipeFromInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                player.GetKitchenObject().SetKitchenObjectParent(this);
                _fryingRecipeSO = GetFryingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                _fryingState = FryingState.Frying;
                _fryingProgress = 0;
                OnFryingStateChange?.Invoke(this, new OnFryingStateChangeEventArgs() {
                    FryingState = _fryingState
                });
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                // Take the kitchen object from the stove
                _fryingState = FryingState.Idle;
                OnFryingStateChange?.Invoke(this, new OnFryingStateChangeEventArgs() {
                    FryingState = _fryingState
                });
                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                    ProgressNormalized = 0
                });
            } else {
                if (player.GetKitchenObject() is PlateKitchenObject plate) {
                    if (plate!.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                        // Take the kitchen object from the stove
                        _fryingState = FryingState.Idle;
                        OnFryingStateChange?.Invoke(this, new OnFryingStateChangeEventArgs() {
                            FryingState = _fryingState
                        });
                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                            ProgressNormalized = 0
                        });
                    }
                } else {
                    Debug.Log("You can't put a " + player.GetKitchenObject() + " on a " + GetKitchenObject() + "!");
                }
            }
        }
    }

    private bool HasRecipeFromInput(KitchenObjectSO kitchenObjectSO) {
        return fryingRecipeSOArray.Any(recipe => recipe.input == kitchenObjectSO);
    }

    private KitchenObjectSO GetOutputFromInput() {
        var kitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();

        // Shouldn't be null
        return fryingRecipeSOArray.First(recipe => recipe.input == kitchenObjectSO).output;
    }

    private FryingRecipeSO GetFryingRecipeSOFromInput(KitchenObjectSO kitchenObjectSO) {
        return fryingRecipeSOArray.FirstOrDefault(recipe => recipe.input == kitchenObjectSO);
    }
}