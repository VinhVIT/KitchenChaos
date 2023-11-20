using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public interface IIngredientParent
{
    public Transform GetIngredientFollowTransform();
    public void SetIngredient(Ingredient ingredient);
    public Ingredient GetIngredient();
    public void ClearIngredient();
    public bool HasIngredient();
    public NetworkObject GetNetworkObject();
}
