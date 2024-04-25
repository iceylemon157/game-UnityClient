using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour {
    public static GameInput Instance { get; private set; }

    public event EventHandler OnInteractAction;
    public event EventHandler OnDropAction;
    public event EventHandler OnInteractAlternativeAction;
    public event EventHandler OnPauseAction;

    private PlayerInputActions _playerInputActions;

    private void Awake() {
        Instance = this;

        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Player.Enable();
        _playerInputActions.Player.Interact.performed += Interact_performed;
        _playerInputActions.Player.InteractAlternative.performed += InteractAlternative_performed;
        _playerInputActions.Player.Drop.performed += Drop_performed;
        _playerInputActions.Player.Pause.performed += Pause_performed;
    }
    
    private void OnDestroy() {
        _playerInputActions.Player.Disable();
        _playerInputActions.Dispose();
    }

    private void Pause_performed(InputAction.CallbackContext obj) {
        OnPauseAction?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Press f for alternative interaction
    /// e.g. cutting a tomato
    /// </summary>
    /// <param name="obj"></param>
    private void InteractAlternative_performed(InputAction.CallbackContext obj) {
        OnInteractAlternativeAction?.Invoke(this, EventArgs.Empty);
    }

    private void Drop_performed(InputAction.CallbackContext obj) {
        OnDropAction?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext obj) {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized() {
        var inputVector = _playerInputActions.Player.Move.ReadValue<Vector2>();
        inputVector = inputVector.normalized;
        return inputVector;
    }
}