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
    private GameData gameData;

    private enum GameState {
        MainMenu,
        Countdown,
        GamePlaying,
        Paused,
        GameOver
    }
    
    private enum RoundState {
        RoundStart,
        RoundPlaying,
        RoundEnd
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
    
    // Round-based game variables
    private int _currentRound;
    private RoundState _roundState;
    private const int TotalRound = 1800;
    
    // Time-based game variables
    private bool _isGamePaused;
    private float _mainMenuTimer = 1f;
    private float _countDownTimer = 3f;
    private float _gamePlayingTimer = 30f;
    private const float TotalGamePlayingTime = 30f;
    
    // Score related variables
    private const int WrongRecipePenalty = -100;
    
    [SerializeField] private StoveCounter stoveCounter;
    
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
        stoveCounter.OnFryingStateChange += StoveCounter_OnFryingStateChange;

        gameData = new GameData();
        
        _currentRound = 1;
        gameData.Round = _currentRound;
        gameData.TotalScore = 0;

    }

    private void StoveCounter_OnFryingStateChange(object sender, StoveCounter.FryingStateChangeEventArgs e) {
        gameData.FryingState = e.FryingState;
    }

    /// <summary>
    /// Update GameData.PlayerHoldItems when player hold item changes
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void Player_OnHoldItemChanged(object sender, EventArgs e) {
        
        if (!Player.Instance.HasKitchenObject()) {
            gameData.PlayerHoldItems = new List<string>();
            return;
        }
        
        gameData.PlayerHoldItems = new List<string> {
            Player.Instance.GetKitchenObject().GetObjectName()
        };

        if (!Player.Instance.GetKitchenObject().TryGetPlate(out var plateKitchenObject)) return;
        // Player is holding a plate
        plateKitchenObject.OnIngredientAdded += Player_OnHoldItemChanged;
        gameData.PlayerHoldItems.AddRange(plateKitchenObject.GetKitchenObjectSOList().Select(x => x.name));
    }

    private void DeliveryManager_OnDeliveryFailed(object sender, EventArgs e) {
        gameData.RecipeDelivered = new List<int> { -1, WrongRecipePenalty };
        gameData.TotalScore += WrongRecipePenalty;
    }

    private void DeliveryManager_OnRecipeCompleted(object sender, DeliveryManager.RecipeEventArgs e) {
        gameData.RecipeDelivered = new List<int> { e.RecipeSO.id, e.RecipeSO.recipeScore };
        gameData.TotalScore += e.RecipeSO.recipeScore;
    }

    private void DeliveryManager_OnRecipeSpawned(object sender, DeliveryManager.RecipeEventArgs e) {
        // Update GameData.NewRecipe and GameData.RecipeList
        gameData.NewRecipe = new RecipeSO.RecipeData() {
            id = e.RecipeSO.id,
            recipeName = e.RecipeSO.recipeName,
            recipeScore = e.RecipeSO.recipeScore,
        };

        gameData.RecipeList = new List<RecipeSO.RecipeData>();
        foreach (var recipeSO in e.RecipeSOList) {
            gameData.RecipeList.Add(new RecipeSO.RecipeData() {
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
                // Time-based game logic
                // _gamePlayingTimer -= Time.deltaTime;
                // if (_gamePlayingTimer <= 0f) {
                //     State = GameState.GameOver;
                // }
                
                // Round-based game logic
                if (_roundState == RoundState.RoundEnd) {
                    _currentRound ++;
                    _roundState = RoundState.RoundStart;
                    
                    gameData.Round = _currentRound;
                }
                if (_currentRound > TotalRound) {
                    State = GameState.GameOver;
                }
                Debug.Log("Current Round: " + _currentRound);
                
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
    
    public bool IsRoundStart() {
        return _roundState == RoundState.RoundStart;
    }
    
    public bool IsRoundPlaying() {
        return _roundState == RoundState.RoundPlaying;
    }
    
    public bool IsRoundEnd() {
        return _roundState == RoundState.RoundEnd;
    }
    
    public void SetRoundPlaying() {
        _roundState = RoundState.RoundPlaying;
    }
    
    public void SetRoundEnd() {
        _roundState = RoundState.RoundEnd;
    }
    
    public int GetCurrentRound() {
        return _currentRound;
    }

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
    
    public float GetGamePlayingRoundNormalized() {
        return (float) _currentRound / TotalRound;
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

    public GameData GetGameData() {
        return gameData;
    }
}