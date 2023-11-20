using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class ContainerCounter : BaseCounter
{   
    [SerializeField] protected IngredientSO IngredientSO;
    public event EventHandler OnSpawnIngredient;

    public override void Interact(Player player)
    {
        if (!player.HasIngredient())
        {
            //spawn ingredient
            Ingredient.SpawnIngredient(IngredientSO,player);

            InteractLogicServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicServerRpc()
    {
        InteractLogicClientRpc();
    }
    [ClientRpc]
    private void InteractLogicClientRpc()
    {
        OnSpawnIngredient?.Invoke(this, EventArgs.Empty);
    }
}
