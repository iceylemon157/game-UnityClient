using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class DeliveryManager : MonoBehaviour {
    public event EventHandler OnDeliveryFailed;
    public event EventHandler OnDeliverySuccess;
    public event EventHandler<OrderEventArgs> OnOrderSpawned;
    public event EventHandler<OrderEventArgs> OnOrderCompleted;

    public class OrderEventArgs : EventArgs {
        public Order Order;
        public List<Order> WaitingOrders;
    }

    public static DeliveryManager Instance { get; private set; }

    [SerializeField] private RecipeListSO recipeListSO;
    private List<Order> _waitingOrders;
    private float _spawnOrderTimer;
    private int _waitingOrdersCount;
    private int _successOrderDelivered;
    private int _mostRecentOrderID;

    private const float SpawnOrderTimeMax = 5f;
    private const int WaitingOrdersMax = 4;

    // Round-based version of game
    private int _previousRound;
    private int _latestSpawnOrderRound;
    private const int SpawnOrderRound = 100;
    private const int MaxOrderScore = 1000;
    
    // Different Recipe Mode
    private const string RecipeModeKey = "RecipeMode";
    private enum RecipeMode {
        Salad,
        SaladAndCheeseBurger,
        AllRecipe
    }
    
    // Seed key
    private const string SeedKey = "Seed";

    private void Awake() {
        Instance = this;
        _waitingOrders = new List<Order>();
    }

    private void Start() {
        _spawnOrderTimer = SpawnOrderTimeMax;
        _waitingOrdersCount = 0;
        _successOrderDelivered = 0;
        _mostRecentOrderID = 0;

        _latestSpawnOrderRound = 0;
        
        // set random seed if any
        var seed = PlayerPrefs.GetInt(SeedKey);
        if (seed != -1) {
            Random.InitState(seed);
        }

        GameManager.Instance.OnNewRound += (_, _) => {
            var currentRound = GameManager.Instance.GetCurrentRound();
            var isAnyOrderUpdated = false;
            foreach (var order in _waitingOrders) {
                var roundElapsed = currentRound - order.ExistedTime;
                if (roundElapsed % 100 != 0) continue;
                isAnyOrderUpdated = true;
                order.OrderScore = roundElapsed switch {
                    100 => 700,
                    200 => 350,
                    _ => 200
                };
            }
            if (isAnyOrderUpdated) {
                OnOrderSpawned?.Invoke(this, new OrderEventArgs {
                    Order = null,
                    WaitingOrders = _waitingOrders
                });
            }
            
            RoundBasedUpdate();
        };
    }

    private void Update() {
        if (GameManager.Instance.IsServerMode()) {
            // RoundBasedUpdate();
        } else {
            TimeBasedUpdate();
        }
    }

    private void RoundBasedUpdate() {
        var currentRound = GameManager.Instance.GetCurrentRound();
        var spawnRecipeRound = currentRound - _latestSpawnOrderRound;
        if (spawnRecipeRound < SpawnOrderRound) return;

        _latestSpawnOrderRound = currentRound;
        CreateNewOrder();
    }

    private void TimeBasedUpdate() {
        _spawnOrderTimer -= Time.deltaTime;
        if (!(_spawnOrderTimer <= 0f)) return;

        _spawnOrderTimer = SpawnOrderTimeMax;
        CreateNewOrder();
    }

    private void CreateNewOrder() {
        if (_waitingOrdersCount >= WaitingOrdersMax) return;
        var currentRound = GameManager.Instance.GetCurrentRound();
        // recipeSOList: Burger, CheeseBurger, MegaBurger, Salad
        var recipeMode = PlayerPrefs.GetString(RecipeModeKey);
        var newRecipeSO = recipeListSO.recipeSOList[Random.Range(0, recipeListSO.recipeSOList.Count)];
        
        if (recipeMode == RecipeMode.SaladAndCheeseBurger.ToString()) {
            var index = 2 * Random.Range(0, 2) + 1;
            newRecipeSO = recipeListSO.recipeSOList[index];
        } else if (recipeMode == RecipeMode.Salad.ToString()) {
            newRecipeSO = recipeListSO.recipeSOList.Last();
        }
        
        var newWaitingOrder = new Order {
            OrderID = ++ _mostRecentOrderID,
            RecipeSO = newRecipeSO,
            OrderScore = MaxOrderScore,
            ExistedTime = currentRound
        };

        // Debug.Log("Order ID: " + newWaitingOrder.OrderID);
        // Debug.Log("Recipe of the order: " + newWaitingOrder.RecipeSO.recipeName);

        _waitingOrders.Add(newWaitingOrder);
        _waitingOrdersCount ++;
        OnOrderSpawned?.Invoke(this, new OrderEventArgs {
            Order = newWaitingOrder,
            WaitingOrders = _waitingOrders
        });
    }

    public bool DeliverOrder(PlateKitchenObject plateKitchenObject) {
        for (var i = 0; i < _waitingOrders.Count; i ++) {
            var waitingOrder = _waitingOrders[i];
            // waitingRecipeSO.kitchenObjectSOList.Sort((x, y) =>
            //     string.Compare(x.objectName, y.objectName, StringComparison.Ordinal));
            plateKitchenObject.GetKitchenObjectSOList().Sort((x, y) =>
                string.Compare(x.objectName, y.objectName, StringComparison.Ordinal));

            var plateContentsMatch =
                waitingOrder.RecipeSO.kitchenObjectSOList.SequenceEqual(plateKitchenObject.GetKitchenObjectSOList());

            if (plateContentsMatch) {
                _waitingOrders.RemoveAt(i);
                _waitingOrdersCount --;
                _successOrderDelivered ++;

                OnOrderCompleted?.Invoke(this, new OrderEventArgs {
                    Order = waitingOrder,
                    WaitingOrders = _waitingOrders
                });
                OnDeliverySuccess?.Invoke(this, EventArgs.Empty);

                return true;
            }
        }

        OnDeliveryFailed?.Invoke(this, EventArgs.Empty);
        return false;
    }

    public List<Order> GetWaitingOrders() {
        return _waitingOrders;
    }

    public int GetSuccessOrderDelivered() {
        return _successOrderDelivered;
    }
}