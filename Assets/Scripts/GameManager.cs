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
    public event EventHandler OnNewRound;

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
    private const int WrongRecipePenalty = 0;
    
    // Game Settings
    private bool _isServerMode = true; // Played by player or server
    private const string IsServerModeKey = "IsServerMode";
    private const string RecipeModeKey = "RecipeMode";
    
    [SerializeField] private StoveCounter stoveCounter;
    
    private void Awake() {
        Instance = this;
        State = GameState.MainMenu;
        
        // Read Game Settings
        _isServerMode = PlayerPrefs.GetInt(IsServerModeKey, 1) == 1;
    }

    private void Start() {
        GameInput.Instance.OnPauseAction += GameInput_OnPauseAction;
        DeliveryManager.Instance.OnOrderSpawned += DeliveryManager_OnOrderSpawned;
        DeliveryManager.Instance.OnOrderCompleted += DeliveryManager_OnOrderCompleted;
        DeliveryManager.Instance.OnDeliveryFailed += DeliveryManager_OnDeliveryFailed;
        Player.Instance.OnHoldItemChanged += Player_OnHoldItemChanged;
        Player.Instance.OnPlayerMoved += Player_OnPlayerMoved;
        stoveCounter.OnFryingStateChange += StoveCounter_OnFryingStateChange;
        stoveCounter.OnProgressChanged += StoveCounter_OnProgressChanged;

        gameData = new GameData();
        
        _currentRound = 1;
        gameData.RecipeMode = PlayerPrefs.GetString(RecipeModeKey);
        gameData.Round = _currentRound;
        gameData.TotalScore = 0;
        gameData.PlayerPosition = Player.Instance.GetPlayerPosition();
        
        // Unpause the game if it was paused
        if (!_isGamePaused) {
            TogglePause();
        }
        
        // AudioListener will play no matter what after restarting, which is strange
        var pause = AudioListener.pause;
        AudioListener.pause = pause;
    }

    private void StoveCounter_OnProgressChanged(object sender, IHasProgress.ProgressChangedEventArgs e) {
        gameData.FryingTimer = e.Progress;
    }

    private void Player_OnPlayerMoved(object sender, EventArgs e) {
        gameData.PlayerPosition = Player.Instance.GetPlayerPosition();
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
            gameData.PlayerHoldItems = new List<int>();
            return;
        }
        
        gameData.PlayerHoldItems = new List<int> {
            Player.Instance.GetKitchenObject().GetKitchenObjectSO().objectID
        };

        if (!Player.Instance.GetKitchenObject().TryGetPlate(out var plateKitchenObject)) return;
        // Player is holding a plate
        plateKitchenObject.OnIngredientAdded += Player_OnHoldItemChanged;
        gameData.PlayerHoldItems.AddRange(plateKitchenObject.GetKitchenObjectSOList().Select(x => x.objectID));
    }

    private void DeliveryManager_OnDeliveryFailed(object sender, EventArgs e) {
        gameData.OrderDelivered = new List<int> { -1, WrongRecipePenalty };
        gameData.TotalScore += WrongRecipePenalty;
    }

    private void DeliveryManager_OnOrderCompleted(object sender, DeliveryManager.OrderEventArgs e) {
        gameData.OrderDelivered = new List<int> { e.Order.OrderID, e.Order.OrderScore };
        gameData.TotalScore += e.Order.OrderScore;
        var orderList = e.WaitingOrders.Select(order => order.GetOrderInfo()).ToList();
        gameData.OrderList = orderList;
    }

    private void DeliveryManager_OnOrderSpawned(object sender, DeliveryManager.OrderEventArgs e) {
        // Update GameData.NewOrder and GameData.OrderList
        if (e.Order != null) {
            // Debug.Log("Round: " + _currentRound);
            // Debug.Log("OnOrderSpawned: " + e.Order.GetOrderInfo().orderID + " - " + e.Order.GetOrderInfo().recipeID + " - " + e.Order.GetOrderInfo().orderScore + " - " + e.Order.GetOrderInfo().existedTime);
            gameData.NewOrder = e.Order.GetOrderInfo();
        }
        
        var orderList = e.WaitingOrders.Select(order => order.GetOrderInfo()).ToList();
        gameData.OrderList = orderList;
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
                
                if (_isServerMode) {
                    // Round-based game logic
                    if (_roundState == RoundState.RoundEnd) {
                        // Debug.Log("--- New Round: " + _currentRound + " ---");
                        _currentRound ++;
                        _roundState = RoundState.RoundStart;
                        OnNewRound?.Invoke(this, EventArgs.Empty);
                        
                        gameData.Round = _currentRound;
                    }
                    if (_currentRound > TotalRound) {
                        State = GameState.GameOver;
                    }
                    // Debug.Log("Current Round: " + _currentRound);
                } else {
                    // Time-based game logic
                    // Infinite game, never game over
                    // _gamePlayingTimer -= Time.deltaTime;
                    if (_gamePlayingTimer <= 0f) {
                        State = GameState.GameOver;
                    }
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
        
        // Debug.Log(State);
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
    
    public void SetServerMode(bool isServerMode) {
        _isServerMode = isServerMode;
    }
    
    public bool IsServerMode() {
        return _isServerMode;
    }

    public void ResetGameData() {
        // Only reset a few game data:
        // OrderDelivered
        gameData.OrderDelivered = new List<int> {0, 0};
        gameData.NewOrder = new Order.OrderInfo {
            orderID = -1
        };
        // Debug.Log("Game Data Reset at Round: " + _currentRound);
    }
    
    public int GetTotalScore() {
        return gameData.TotalScore;
    }
}