using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameInput : MonoBehaviour
{
    private const string PLAYER_PREF_BINDINGS = "InputBindings";
    public static GameInput Instance;
    public event EventHandler OnInteractAction;
    public event EventHandler OnInteract2Action;
    public event EventHandler OnPauseGame;

    public enum Binding
    {
        MoveUp, MoveDown, MoveLeft, MoveRight, PickUp, Cut, Pause
    }
    private PlayerInputAction playerInputAction;
    private void Awake()
    {
        Instance = this;

        playerInputAction = new PlayerInputAction();

        if (PlayerPrefs.HasKey(PLAYER_PREF_BINDINGS))
        {
            playerInputAction.LoadBindingOverridesFromJson(PlayerPrefs.GetString(PLAYER_PREF_BINDINGS));
        }

        playerInputAction.Player.Enable();
        playerInputAction.Player.Interact.performed += Interact_performed;
        playerInputAction.Player.Interact2.performed += Interact2_performed;
        playerInputAction.Player.Pause.performed += Pause_performed;
    }
    private void OnDestroy()
    {
        playerInputAction.Player.Interact.performed -= Interact_performed;
        playerInputAction.Player.Interact2.performed -= Interact2_performed;
        playerInputAction.Player.Pause.performed -= Pause_performed;

        playerInputAction.Dispose();// clear memories
    }
    private void Pause_performed(InputAction.CallbackContext context)
    {
        OnPauseGame?.Invoke(this, EventArgs.Empty);
    }

    private void Interact2_performed(InputAction.CallbackContext context)
    {
        OnInteract2Action?.Invoke(this, EventArgs.Empty);
    }

    private void Interact_performed(InputAction.CallbackContext obj)
    {
        OnInteractAction?.Invoke(this, EventArgs.Empty);
    }

    public Vector2 GetMovementVectorNormalized()
    {
        Vector2 input = playerInputAction.Player.Movement.ReadValue<Vector2>();
        input = input.normalized;
        return input;
    }
    public string GetBindingInputText(Binding binding)
    {
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                return playerInputAction.Player.Movement.bindings[1].ToDisplayString();
            case Binding.MoveDown:
                return playerInputAction.Player.Movement.bindings[2].ToDisplayString();
            case Binding.MoveLeft:
                return playerInputAction.Player.Movement.bindings[3].ToDisplayString();
            case Binding.MoveRight:
                return playerInputAction.Player.Movement.bindings[4].ToDisplayString();
            case Binding.PickUp:
                return playerInputAction.Player.Interact.bindings[0].ToDisplayString();
            case Binding.Cut:
                return playerInputAction.Player.Interact2.bindings[0].ToDisplayString();
            case Binding.Pause:
                return playerInputAction.Player.Pause.bindings[0].ToDisplayString();
        }
    }
    public void RebindBinding(Binding binding, Action onActionRebound)
    {
        playerInputAction.Player.Disable();//disable before rebinding
        InputAction inputAction;
        int bindingIndex;
        switch (binding)
        {
            default:
            case Binding.MoveUp:
                inputAction = playerInputAction.Player.Movement;
                bindingIndex = 1;
                break;
            case Binding.MoveDown:
                inputAction = playerInputAction.Player.Movement;
                bindingIndex = 2;
                break;
            case Binding.MoveLeft:
                inputAction = playerInputAction.Player.Movement;
                bindingIndex = 3;
                break;
            case Binding.MoveRight:
                inputAction = playerInputAction.Player.Movement;
                bindingIndex = 4;
                break;
            case Binding.PickUp:
                inputAction = playerInputAction.Player.Interact;
                bindingIndex = 0;
                break;
            case Binding.Cut:
                inputAction = playerInputAction.Player.Interact2;
                bindingIndex = 0;
                break;
            case Binding.Pause:
                inputAction = playerInputAction.Player.Pause;
                bindingIndex = 0;
                break;
        }
        inputAction.PerformInteractiveRebinding(bindingIndex).OnComplete(callback =>
        {
            playerInputAction.Player.Enable();//enable after rebinding
            onActionRebound();//Delegate func
            PlayerPrefs.SetString(PLAYER_PREF_BINDINGS, playerInputAction.SaveBindingOverridesAsJson());
            PlayerPrefs.Save();
        })
        .Start();
    }
}
