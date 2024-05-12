using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {
    public event EventHandler OnDeliveryFailed;
    public event EventHandler OnDeliverySuccess;
    public event EventHandler<RecipeEventArgs> OnRecipeSpawned;
    public event EventHandler<RecipeEventArgs> OnRecipeCompleted;

    public class RecipeEventArgs : EventArgs {
        public RecipeSO RecipeSO;
        public List<RecipeSO> RecipeSOList;
    }

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    private List<RecipeSO> _waitingRecipeSOs;
    private float _spawnRecipeTimer;
    private int _waitingRecipesCount;
    private int _successRecipeDelivered;
    private int _mostRecentRecipeID;

    private const float SpawnRecipeTimeMax = 5f;
    private const int WaitingRecipesMax = 4;
    
    // Round-based version of game
    private int _latestSpawnRecipeRound;
    private const int SpawnRecipeRound = 200;

    private void Awake() {
        Instance = this;
        _waitingRecipeSOs = new List<RecipeSO>();
    }

    private void Start() {
        _spawnRecipeTimer = SpawnRecipeTimeMax;
        _waitingRecipesCount = 0;
        _successRecipeDelivered = 0;
        _mostRecentRecipeID = 0;
        
        _latestSpawnRecipeRound = 0;
    }
    
    private void Update() {
        Debug.Log("I don't know " + OnRecipeCompleted!.GetInvocationList().Length);
        if (GameManager.Instance.IsServerMode()) {
            RoundBasedUpdate();
        } else {
            TimeBasedUpdate();
        }
    }

    private void RoundBasedUpdate() {
        var currentRound = GameManager.Instance.GetCurrentRound();
        var spawnRecipeRound = currentRound - _latestSpawnRecipeRound;
        if (spawnRecipeRound < SpawnRecipeRound) return;
        
        _latestSpawnRecipeRound = currentRound;
        CreateNewRecipe();
    }
    
    private void TimeBasedUpdate() {
        _spawnRecipeTimer -= Time.deltaTime;
        if (!(_spawnRecipeTimer <= 0f)) return;
        
        _spawnRecipeTimer = SpawnRecipeTimeMax;
        CreateNewRecipe();
    }
    
    private void CreateNewRecipe() {
        if (_waitingRecipesCount >= WaitingRecipesMax) return;
        // Don't fix the yellow warning
        // Or otherwise you will have to copy the RecipeSO field one by one
        var waitingRecipeSO = new RecipeSO(recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)]);
        waitingRecipeSO.id = ++ _mostRecentRecipeID;
        Debug.Log("New recipe: " + waitingRecipeSO.recipeName + " is waiting!");
        Debug.Log("Recipe ID: " + waitingRecipeSO.id);
        _waitingRecipeSOs.Add(waitingRecipeSO);
        _waitingRecipesCount ++;
        OnRecipeSpawned?.Invoke(this, new RecipeEventArgs() {
            RecipeSO = waitingRecipeSO,
            RecipeSOList = _waitingRecipeSOs
        });
    }

    public bool DeliverRecipe(PlateKitchenObject plateKitchenObject) {
        for (var i = 0; i < _waitingRecipeSOs.Count; i ++) {
            var waitingRecipeSO = _waitingRecipeSOs[i];
            // waitingRecipeSO.kitchenObjectSOList.Sort((x, y) =>
            //     string.Compare(x.objectName, y.objectName, StringComparison.Ordinal));
            plateKitchenObject.GetKitchenObjectSOList().Sort((x, y) =>
                string.Compare(x.objectName, y.objectName, StringComparison.Ordinal));

            var plateContentsMatch =
                waitingRecipeSO.kitchenObjectSOList.SequenceEqual(plateKitchenObject.GetKitchenObjectSOList());

            if (plateContentsMatch) {
                _waitingRecipeSOs.RemoveAt(i);
                _waitingRecipesCount --;
                _successRecipeDelivered ++;
                
                OnRecipeCompleted?.Invoke(this, new RecipeEventArgs() {
                    RecipeSO = waitingRecipeSO,
                    RecipeSOList = _waitingRecipeSOs
                });
                OnDeliverySuccess?.Invoke(this, EventArgs.Empty);
                
                return true;
            }
        }

        OnDeliveryFailed?.Invoke(this, EventArgs.Empty);
        return false;
    }

    public List<RecipeSO> GetWaitingRecipeSOs() {
        return _waitingRecipeSOs;
    }

    public int GetSuccessRecipeDelivered() {
        return _successRecipeDelivered;
    }
}