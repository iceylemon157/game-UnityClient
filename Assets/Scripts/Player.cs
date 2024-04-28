using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : MonoBehaviour, IKitchenObjectParent {
    // Singleton
    public static Player Instance { get; private set; }

    public event EventHandler OnPlayerMoved;
    public event EventHandler OnPickedUpSomething;
    public event EventHandler OnHoldItemChanged;
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class SelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter SelectedCounter;
    }

    [SerializeField] private int port = 8887;
    [SerializeField] private bool testMode = true;
    [SerializeField] private float moveSpeed = 10f;
    [SerializeField] private float rotationSpeed = 8f;
    // [SerializeField] private GameInput gameInput;
    // [SerializeField] private LayerMask counterLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool _isWalking;
    private Vector3 _lastInteractionDirection;
    private BaseCounter _selectedCounter;
    private KitchenObject _kitchenObject;

    private const float PlayerSize = .65f;
    private const float PlayerHeight = 2f;
    private const float InteractionDistance = 2f;
    private const float MoveDistance = .5f;
    private const float MapBasedRotationSpeed = 20f;
    
    // Round-based game variables
    private bool roundEnd;
    
    // Map related game variables
    private static readonly Vector2 InitialPlayerPosition = new Vector2(4, 10);
    private Vector2 _playerPosition;

    private void Awake() {
        if (Instance != null) {
            Debug.Log("Player instance already exists!");
        }
        
        Instance = this;
        
        // Map based player position
        _playerPosition = InitialPlayerPosition;
    }

    private void Start() {
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
        GameInput.Instance.OnInteractAlternativeAction += GameInput_OnInteractAlternativeAction;
        GameInput.Instance.OnDropAction += GameInput_OnDropAction;

        
        // send an initial message to the server
        StartCoroutine(SendInitialMessageToServer());
    }

    private void GameInput_OnInteractAlternativeAction(object sender, EventArgs e) {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (_selectedCounter != null) {
            _selectedCounter.InteractAlternative(this);
        }
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e) {
        if (!GameManager.Instance.IsGamePlaying()) return;
        if (_selectedCounter != null) {
            _selectedCounter.Interact(this);
        }
    }

    private void GameInput_OnDropAction(object sender, EventArgs e) {
        // press q to drop the kitchen object with constant speed
        Debug.Log("You have pressed the drop button!");
        if (!HasKitchenObject()) return;
        Debug.Log("Kitchen object: " + GetKitchenObject() + "is being dropped!");
        var kitchenObject = GetKitchenObject();
        ClearKitchenObject();
        kitchenObject.FallWithConstantSpeed();
    }

    private void Update() {
        
        // TODO: Determine if the player can move or not when counting down?
        // if (!GameManager.Instance.IsGamePlaying() && !GameManager.Instance.IsCountDown()) return;
        if (!GameManager.Instance.IsGamePlaying()) return;

        
        if (GameManager.Instance.IsRoundStart()) {
            GameManager.Instance.SetRoundPlaying();
            roundEnd = false;
            if (!testMode) {
                var inputVector = GameInput.Instance.GetMovementVectorNormalized();
                HandleMovement(inputVector);
                HandleInteractions(inputVector);
            } else {
                StartCoroutine(GetOperationFromServer(movementVector => {
                    // Debug.Log("Received movement vector from server!" + movementVector);
                    // Debug.Log("Operation sent and received!");
                    MapBasedHandleMovement(movementVector);
                    HandleInteractions(movementVector);
                    // Debug.Log("Ready to send events to server!");
                    SendEventsToServer();
                    roundEnd = true;
                }));
                // StartCoroutine(SendEventsRequestToServer());
            }
        }
        
        if (testMode && roundEnd) {
            GameManager.Instance.SetRoundEnd();
        }
    }

    private bool CanMove(Vector3 origin, Vector3 moveDir, float moveDistance) {
        return !Physics.CapsuleCast(origin, origin + Vector3.up * PlayerHeight,
            PlayerSize, moveDir, moveDistance);
    }
    
    private void SendEventsToServer() {
        StartCoroutine(SendEventsRequestToServer());
    }
    
    private IEnumerator SendEventsRequestToServer() {
        
        var url = "http://127.0.0.1:" + port + "/api/events";

        var request = new UnityWebRequest(url, "POST");
        request.SetRequestHeader("Content-Type", "application/json");

        // var gameData = GetGameData();
        var gameData = GameManager.Instance.GetGameData();
        var gameDataJsonString = JsonUtility.ToJson(gameData);
        
        var bodyRaw = System.Text.Encoding.UTF8.GetBytes(gameDataJsonString);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        
        yield return request.SendWebRequest();
    }
    
    private static GameData GetGameData() {
        var gameData = new GameData {
            Round = 0,
            TotalScore = Convert.ToInt32(GameManager.Instance.GetGamePlayingTimerNormalized() * 1000),
            TimeLeft = 0,
            
            NewRecipe = null,
            RecipeDelivered = new List<int> {0, 0},
            RecipeTimeout = 0,
            RecipeList = new List<RecipeSO.RecipeData>(),
            
            PlayerPosition = new Vector2(0, 0),
            PlayerHoldItems = null,
            
            FryingTimer = 0,
            FryingState = 0,
        };
        return gameData;
    }

    private IEnumerator GetOperationFromServer(Action<Vector2> callback) {
        
        var currentRound = GameManager.Instance.GetCurrentRound();
        var webRequest = UnityWebRequest.Get("http://127.0.0.1:" + port + $"/api/operation?round={currentRound}");
        yield return webRequest.SendWebRequest();
        
        // Debug.Log("Received response from server...");

        if (webRequest.result != UnityWebRequest.Result.Success) {
            Debug.Log(webRequest.error);
            // exit the coroutine
            callback(Vector2.zero);
        }

        var result = webRequest.downloadHandler.text;
        webRequest.Dispose();
        
        Debug.Log("Operation received from server: " + result);
        
        switch (result) {
            case "w":
                callback(Vector2.up);
                break;
            case "s":
                callback(Vector2.down);
                break;
            case "a":
                callback(Vector2.left);
                break;
            case "d":
                callback(Vector2.right);
                break;
            case "e":
                GameInput_OnInteractAction(null, EventArgs.Empty);
                break;
            case "f":
                GameInput_OnInteractAlternativeAction(null, EventArgs.Empty);
                break;
        }
        
        // must call the callback function, for the coroutine to exit
        callback(Vector2.zero);
    }
    
    private IEnumerator SendInitialMessageToServer() {
        // This function should be called when each time the game starts
        var webRequest = UnityWebRequest.Get("http://127.0.0.1:" + port + "/api/init");
        yield return webRequest.SendWebRequest();
        
        if (webRequest.result != UnityWebRequest.Result.Success) {
            Debug.Log(webRequest.error);
        }
        
        var result = webRequest.downloadHandler.text;
        if (result == "ok") {
            Debug.Log("Initial message sent to the server!");
            Debug.Log("Server seems to be ready!");
        } else {
            Debug.Log("Your server fucked up!");
            Debug.Log($"This is the message from server: {result}!");
        }
        
        webRequest.Dispose();
    }
    
    private void MapBasedHandleMovement(Vector2 inputVector) {
        // In map based (game client mode), player can only walk alone grid
        var moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        var transform1 = transform;
        transform1.forward = Vector3.Slerp(transform1.forward, moveDir, Time.deltaTime * MapBasedRotationSpeed);

        _isWalking = (inputVector != Vector2.zero);

        var originalPosition = transform.position;
        var canMove = CanMove(originalPosition, moveDir, MoveDistance);
        if (!canMove) return;
        
        // Update Map-based player position
        var mapBasedMoveDirection = new Vector2(-inputVector.y, inputVector.x);
        _playerPosition += mapBasedMoveDirection;
        OnPlayerMoved?.Invoke(this, EventArgs.Empty);
        
        transform1.position += moveDir * MoveDistance;
    }

    private void HandleMovement(Vector2 inputVector) {
        var moveDir = new Vector3(inputVector.x, 0, inputVector.y);

        var transform1 = transform;
        transform1.forward = Vector3.Slerp(transform1.forward, moveDir, Time.deltaTime * rotationSpeed);

        _isWalking = (inputVector != Vector2.zero);

        var moveDistance = moveSpeed * Time.deltaTime;
        var originalPosition = transform.position;
        var canMove = CanMove(originalPosition, moveDir, moveDistance);

        if (!canMove) {
            var moveDirX = new Vector3(moveDir.x, 0, 0);
            var moveDirZ = new Vector3(0, 0, moveDir.z);
            if (CanMove(originalPosition, moveDirX, moveDistance)) {
                moveDir = moveDirX;
                canMove = true;
            } else if (CanMove(originalPosition, moveDirZ, moveDistance)) {
                moveDir = moveDirZ;
                canMove = true;
            }
        }

        if (canMove) {
            Debug.Log("Move distance: " + moveDistance);
            transform1.position += moveDir * moveDistance;
        }
    }

    /// <summary>
    /// Determine the selected counter by ray-casting
    /// </summary>
    /// <param name="inputVector"></param>
    private void HandleInteractions(Vector2 inputVector) {
        var origin = transform.position;
        var moveDir = new Vector3(inputVector.x, 0, inputVector.y);
        if (moveDir != Vector3.zero) {
            _lastInteractionDirection = moveDir;
        }
        // Debug.Log("moveDir" + moveDir);

        // Only the case of the selected counter is the previous counter will not reselect the counter
        if (Physics.Raycast(origin, _lastInteractionDirection, out RaycastHit raycastHit, InteractionDistance)) {
            if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
                if (_selectedCounter != baseCounter) {
                    SetSelectedCounter(baseCounter);
                }
            } else {
                SetSelectedCounter(null);
            }
        } else {
            SetSelectedCounter(null);
        }
        // Debug.Log(selectedCounter);
    }

    private void SetSelectedCounter(BaseCounter baseCounter) {
        _selectedCounter = baseCounter;
        OnSelectedCounterChanged?.Invoke(this, new SelectedCounterChangedEventArgs() {
            SelectedCounter = _selectedCounter
        });
    }

    public bool IsWalking() {
        return _isWalking;
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public KitchenObject GetKitchenObject() {
        return _kitchenObject;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        _kitchenObject = kitchenObject;
        OnHoldItemChanged?.Invoke(this, EventArgs.Empty);
        if (HasKitchenObject()) {
            OnPickedUpSomething?.Invoke(this, EventArgs.Empty);
        } else {
            // Should never come here
            Debug.LogError("Error: Kitchen object is null! Use ClearKitchenObject() instead!");
        }
    }

    public void ClearKitchenObject() {
        if (!HasKitchenObject()) return;
        _kitchenObject = null;
        OnHoldItemChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool HasKitchenObject() {
        return _kitchenObject != null;
    }
    
    public Vector2 GetPlayerPosition() {
        return _playerPosition;
    }
}