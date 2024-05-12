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

    public event EventHandler<FryingStateChangeEventArgs> OnFryingStateChange;

    public class FryingStateChangeEventArgs : EventArgs {
        public FryingState FryingState;
    }

    // Timer is for time-based version of game
    private float _fryingTimer;
    
    // Round-based version of game
    private int _fryingRound;
    private int _startFryingRound;

    private FryingRecipeSO _fryingRecipeSO;
    private FryingState _fryingState;

    private void Start() {
        _fryingState = FryingState.Idle;
        _fryingTimer = 0;
        _fryingRound = 0;
        _startFryingRound = 0;
    }
    
    private void Update() {
        if (GameManager.Instance.IsServerMode()) {
            RoundBasedUpdate();
        } else {
            TimeBasedUpdate();
        }
    }
    
    private void RoundBasedUpdate() {
        if (HasKitchenObject()) {
            switch (_fryingState) {
                case FryingState.Idle:
                    break;
                case FryingState.Frying:
                    _fryingRound = GameManager.Instance.GetCurrentRound() - _startFryingRound;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                        ProgressNormalized = _fryingRound / _fryingRecipeSO.fryingRound
                    });
                    if (_fryingRound >= _fryingRecipeSO.fryingRound) {
                        _startFryingRound = GameManager.Instance.GetCurrentRound();
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);

                        _fryingState = FryingState.Fried;
                        OnFryingStateChange?.Invoke(this, new FryingStateChangeEventArgs() {
                            FryingState = _fryingState
                        });
                        _fryingRecipeSO = GetFryingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                    }

                    break;
                case FryingState.Fried:
                    _fryingRound = GameManager.Instance.GetCurrentRound() - _startFryingRound;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                        ProgressNormalized = _fryingRound / _fryingRecipeSO.fryingRound
                    });
                    if (_fryingRound >= _fryingRecipeSO.fryingRound) {
                        _startFryingRound = -1;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);
                        _fryingState = FryingState.Burnt;
                        OnFryingStateChange?.Invoke(this, new FryingStateChangeEventArgs() {
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

    private void TimeBasedUpdate() {
        if (HasKitchenObject()) {
            switch (_fryingState) {
                case FryingState.Idle:
                    break;
                case FryingState.Frying:
                    _fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                        ProgressNormalized = 1f * _fryingTimer / _fryingRecipeSO.fryingTime
                    });
                    if (_fryingTimer >= _fryingRecipeSO.fryingTime) {
                        _fryingTimer = 0;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);

                        _fryingState = FryingState.Fried;
                        OnFryingStateChange?.Invoke(this, new FryingStateChangeEventArgs() {
                            FryingState = _fryingState
                        });
                        _fryingRecipeSO = GetFryingRecipeSOFromInput(GetKitchenObject().GetKitchenObjectSO());
                    }

                    break;
                case FryingState.Fried:
                    _fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs() {
                        ProgressNormalized = 1f * _fryingTimer / _fryingRecipeSO.fryingTime
                    });
                    if (_fryingTimer >= _fryingRecipeSO.fryingTime) {
                        _fryingTimer = 0;
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(_fryingRecipeSO.output, this);
                        _fryingState = FryingState.Burnt;
                        OnFryingStateChange?.Invoke(this, new FryingStateChangeEventArgs() {
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
                _fryingTimer = 0;
                _startFryingRound = GameManager.Instance.GetCurrentRound();
                OnFryingStateChange?.Invoke(this, new FryingStateChangeEventArgs() {
                    FryingState = _fryingState
                });
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                // Take the kitchen object from the stove
                _fryingState = FryingState.Idle;
                OnFryingStateChange?.Invoke(this, new FryingStateChangeEventArgs() {
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
                        OnFryingStateChange?.Invoke(this, new FryingStateChangeEventArgs() {
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

    public FryingState GetFryingState() {
        return _fryingState;
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