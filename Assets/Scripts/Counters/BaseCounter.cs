using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BaseCounter : NetworkBehaviour, IIngredientParent
{
    public static event EventHandler OnSomethingPlacedHere;
    public static void ResetStaticData() { OnSomethingPlacedHere = null; }
    [SerializeField] protected Transform foodHoldPoint;
    private Ingredient ingredient;
    public virtual void Interact(Player player)
    {
    }
    public virtual void Interact2(Player player)
    {
    }
    public Transform GetIngredientFollowTransform()
    {
        return foodHoldPoint;
    }
    public void SetIngredient(Ingredient ingredient)
    {
        this.ingredient = ingredient;
        if (ingredient != null)
        {
            OnSomethingPlacedHere?.Invoke(this, EventArgs.Empty);
        }
    }
    public Ingredient GetIngredient() => ingredient;
    public void ClearIngredient() => ingredient = null;
    public bool HasIngredient()
    {
        return ingredient != null;
    }
    public NetworkObject GetNetworkObject()
    {
        return NetworkObject;
    }
}
