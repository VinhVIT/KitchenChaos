using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class StoveCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangedEventArg> OnProgressChanged;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs
    {
        public State state;
    }
    public enum State
    {
        Idle,
        Frying,
        Fried,
        Burned
    }
    [SerializeField] private FryingRecipeSO[] fryingRecipeSOArray;
    [SerializeField] private BurningRecipeSO[] burningRecipeSOArray;
    private NetworkVariable<State> state = new NetworkVariable<State>(State.Idle);
    private NetworkVariable<float> fryingTimer = new NetworkVariable<float>(0f);
    private NetworkVariable<float> burningTimer = new NetworkVariable<float>(0f);
    private FryingRecipeSO fryingRecipeSO;
    private BurningRecipeSO burningRecipeSO;

    public override void OnNetworkSpawn()
    {
        fryingTimer.OnValueChanged += FryingTimer_OnValueChanged;
        burningTimer.OnValueChanged += BurningTimer_OnValueChanged;
        state.OnValueChanged += State_OnValueChanged;
    }

    private void BurningTimer_OnValueChanged(float previousValue, float newValue)
    {
        float maxBurningTimer = burningRecipeSO != null ? burningRecipeSO.maxBurningTimer : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArg
        {
            currentProgess = burningTimer.Value / maxBurningTimer
        });
    }

    private void FryingTimer_OnValueChanged(float previousValue, float newValue)
    {
        float maxFryingTimer = fryingRecipeSO != null ? fryingRecipeSO.maxFryingTimer : 1f;
        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArg
        {
            currentProgess = fryingTimer.Value / maxFryingTimer
        });
    }
    private void State_OnValueChanged(State previousState, State newState)
    {
        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs
        {
            state = state.Value
        });
        if (state.Value == State.Burned || state.Value == State.Idle)
        {
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArg
            {
                currentProgess = 0f
            });
        }
    }
    private void Update()
    {
        if (!IsServer) return;
        if (HasIngredient())
        {
            switch (state.Value)
            {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer.Value += Time.deltaTime;

                    if (fryingTimer.Value > fryingRecipeSO.maxFryingTimer)
                    {
                        //Fried
                        Ingredient.DestroyIngredient(GetIngredient());
                        Ingredient.SpawnIngredient(fryingRecipeSO.output, this);

                        state.Value = State.Fried;

                        burningTimer.Value = 0;
                        SetBuringRecipeSOClientRpc(KitchenGameMultiplayer.Instance.GetIngredientSOIndex(GetIngredient().GetIngredientSO()));
                    }
                    break;
                case State.Fried:
                    burningTimer.Value += Time.deltaTime;

                    if (burningTimer.Value > burningRecipeSO.maxBurningTimer)
                    {
                        //Burned
                        Ingredient.DestroyIngredient(GetIngredient());
                        Ingredient.SpawnIngredient(burningRecipeSO.output, this);

                        state.Value = State.Burned;
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }
    public override void Interact(Player player)
    {
        if (!HasIngredient())
        //there no ingredient on table
        {
            if (player.HasIngredient())
            {
                //Check if that ingredient can fry or not 
                if (HasRecipeInput(player.GetIngredient().GetIngredientSO()))
                {
                    Ingredient ingredient = player.GetIngredient();
                    //put ingredient on the table 
                    ingredient.SetIngredientParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc(KitchenGameMultiplayer.Instance.GetIngredientSOIndex(ingredient.GetIngredientSO()));

                }
            }
            else
            {
                //do notthing
            }
        }
        else
        //had ingredient on table
        {
            if (player.HasIngredient())
            {
                if (player.GetIngredient().TryGetPlate(out Plate plate))
                //Check if player holding plate
                {
                    if (plate.TryAddIngredient(GetIngredient().GetIngredientSO()))
                    {
                        Ingredient.DestroyIngredient(GetIngredient());

                        SetStateIdleServerRpc();

                    }
                }
            }
            else
            {
                GetIngredient().SetIngredientParent(player);

                SetStateIdleServerRpc();
            }
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetStateIdleServerRpc() {
        state.Value = State.Idle;
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc(int ingredientSOIndex)
    {
        fryingTimer.Value = 0f;
        state.Value = State.Frying;

        SetFryingRecipeSOClientRpc(ingredientSOIndex);
    }
    [ClientRpc]
    private void SetFryingRecipeSOClientRpc(int ingredientSOIndex)
    {
        IngredientSO ingredientSO = KitchenGameMultiplayer.Instance.GetIngredientSOFromIndex(ingredientSOIndex);
        fryingRecipeSO = GetFryingRecipeSOWithInput(ingredientSO);
    }
    [ClientRpc]
    private void SetBuringRecipeSOClientRpc(int ingredientSOIndex)
    {
        IngredientSO ingredientSO = KitchenGameMultiplayer.Instance.GetIngredientSOFromIndex(ingredientSOIndex);
        burningRecipeSO = GetBurningRecipeSOWithInput(ingredientSO);
    }
    private bool HasRecipeInput(IngredientSO inputIngredientSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputIngredientSO);

        return fryingRecipeSO != null;
    }
    private IngredientSO GetOutPutForInput(IngredientSO inputIngredientSO)
    {
        FryingRecipeSO fryingRecipeSO = GetFryingRecipeSOWithInput(inputIngredientSO);
        if (fryingRecipeSO != null)
        {
            return fryingRecipeSO.output;
        }
        else
        {
            return null;
        }
    }
    private FryingRecipeSO GetFryingRecipeSOWithInput(IngredientSO inputIngredientSO)
    {
        foreach (FryingRecipeSO fryingRecipeSO in fryingRecipeSOArray)
        {
            if (fryingRecipeSO.input == inputIngredientSO)
            {
                return fryingRecipeSO;
            }
        }
        return null;
    }
    private BurningRecipeSO GetBurningRecipeSOWithInput(IngredientSO inputIngredientSO)
    {
        foreach (BurningRecipeSO burningRecipeSO in burningRecipeSOArray)
        {
            if (burningRecipeSO.input == inputIngredientSO)
            {
                return burningRecipeSO;
            }
        }
        return null;
    }
    public bool isFried()
    {
        return state.Value == State.Fried;
    }
}
