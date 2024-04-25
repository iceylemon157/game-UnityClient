using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager Instance { get; private set; }

    public event EventHandler OnGameStateChanged;
    public event EventHandler OnGamePaused;
    public event EventHandler OnGameUnPaused;

    // Global GameData
    public GameData GameData;

    private enum GameState {
        MainMenu,
        Countdown,
        GamePlaying,
        Paused,
        GameOver
    }

    private GameState _gameState;

    private GameState _stateBeforePause;

    private GameState State {
        get => _gameState;
        set {
            _gameState = value;
            OnGameStateChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private bool _isGamePaused;
    private float _mainMenuTimer = 1f;
    private float _countDownTimer = 3f;
    private float _gamePlayingTimer = 30f;
    private const float TotalGamePlayingTime = 30f;
    private const int WrongRecipePenalty = -100;

    private void Awake() {
        Instance = this;
        State = GameState.MainMenu;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManager_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeCompleted;
        DeliveryManager.Instance.OnDeliveryFailed += DeliveryManager_OnDeliveryFailed;
        Player.Instance.OnHoldItemChanged += Player_OnHoldItemChanged;

        GameData = new GameData();
    }

    public void Player_OnHoldItemChanged(object sender, EventArgs e) {
        // Should this not be public?
        
        if (!Player.Instance.HasKitchenObject()) {
            GameData.PlayerHoldItems = new List<string>();
            return;
        }
        
        GameData.PlayerHoldItems = new List<string> {
            Player.Instance.GetKitchenObject().GetObjectName()
        };
        
        if (Player.Instance.GetKitchenObject().TryGetPlate(out var plateKitchenObject)) {
            // Player is holding a plate
            // plateKitchenObject.OnIngredientAdded += PlateKitchenObject_OnIngredientAdded;
            GameData.PlayerHoldItems.AddRange(plateKitchenObject.GetKitchenObjectSOList().Select(x => x.name));
        }
    }

    private void DeliveryManager_OnDeliveryFailed(object sender, EventArgs e) {
        GameData.RecipeDelivered = new List<int> { -1, WrongRecipePenalty };
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, DeliveryManager.RecipeEventArgs e) {
        GameData.RecipeDelivered = new List<int> { e.RecipeSO.id, e.RecipeSO.recipeScore };
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, DeliveryManager.RecipeEventArgs e) {
        // Update GameData.NewRecipe and GameData.RecipeList
        GameData.NewRecipe = new RecipeSO.RecipeData() {
            id = e.RecipeSO.id,
            recipeName = e.RecipeSO.recipeName,
            recipeScore = e.RecipeSO.recipeScore,
        };

        GameData.RecipeList = new List<RecipeSO.RecipeData>();
        foreach (var recipeSO in e.RecipeSOList) {
            GameData.RecipeList.Add(new RecipeSO.RecipeData() {
                id = recipeSO.id,
                recipeName = recipeSO.recipeName,
                recipeScore = recipeSO.recipeScore,
            });
        }
    }

    private void GameInput_OnPauseAction(object sender, EventArgs e) {
        TogglePause();
    }

    private void Update() {
        switch (State) {
            case GameState.MainMenu:
                _mainMenuTimer -= Time.deltaTime;
                if (_mainMenuTimer <= 0f) {
                    State = GameState.Countdown;
                }

                break;
            case GameState.Countdown:
                _countDownTimer -= Time.deltaTime;
                if (_countDownTimer <= 0f) {
                    State = GameState.GamePlaying;
                }

                break;
            case GameState.GamePlaying:
                _gamePlayingTimer -= Time.deltaTime;
                if (_gamePlayingTimer <= 0f) {
                    State = GameState.GameOver;
                }

                break;
            case GameState.Paused:
                break;
            case GameState.GameOver:
                // do nothing
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        Debug.Log(State);
    }

    // public void StartGame() {
    //     State = GameState.Countdown;
    // }

    public bool IsGamePlaying() {
        return State == GameState.GamePlaying;
    }

    public bool IsCountDown() {
        return State == GameState.Countdown;
    }

    public bool IsGameOver() {
        return State == GameState.GameOver;
    }

    public float GetCountDownTimer() {
        return _countDownTimer;
    }

    public float GetGamePlayingTimerNormalized() {
        return 1 - _gamePlayingTimer / TotalGamePlayingTime;
    }

    public void TogglePause() {
        _isGamePaused = !_isGamePaused;
        if (_isGamePaused) {
            Time.timeScale = 1f;
            State = _stateBeforePause;
            OnGameUnPaused?.Invoke(this, EventArgs.Empty);
        } else {
            Time.timeScale = 0f;
            _stateBeforePause = State;
            State = GameState.Paused;
            OnGamePaused?.Invoke(this, EventArgs.Empty);
        }
    }
}