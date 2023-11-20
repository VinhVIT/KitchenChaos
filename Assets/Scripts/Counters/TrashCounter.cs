using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TrashCounter : BaseCounter
{
    public static event EventHandler OnReceiveTrash;
    new static void ResetStaticData() { OnReceiveTrash = null; }
    public override void Interact(Player player)
    {
        if (player.HasIngredient())
        {   
            Ingredient.DestroyIngredient(player.GetIngredient());

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
        OnReceiveTrash?.Invoke(this, EventArgs.Empty);
    }
}
