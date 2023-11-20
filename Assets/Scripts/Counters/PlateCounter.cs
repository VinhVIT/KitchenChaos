using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class PlateCounter : BaseCounter
{
    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;
    [SerializeField] protected IngredientSO IngredientSO;
    private float plateSpawningTimer;
    private float maxplateSpawningTimer = 6f;
    private int plateSpawnAmount;
    private int maxPlateSpawnAmount = 4;

    private void Update()
    {
        if (!IsOwner) return;
        SpawnPlate();
    }
    private void SpawnPlate()
    {
        plateSpawningTimer += Time.deltaTime;
        if (GameManager.Instance.IsPlaying() && plateSpawningTimer > maxplateSpawningTimer)
        //Check plate Spawner Time
        {
            plateSpawningTimer = 0f;
            if (plateSpawnAmount < maxPlateSpawnAmount)
            {
                SpawnPlateServerRpc();
            }
        }
    }
    [ServerRpc]
    private void SpawnPlateServerRpc()
    {
        SpawnPlateClientRpc();
    }
    [ClientRpc]
    private void SpawnPlateClientRpc()
    {
        plateSpawnAmount++;
        OnPlateSpawned?.Invoke(this, EventArgs.Empty);
    }
    public override void Interact(Player player)
    {
        if (!player.HasIngredient())
        {
            if (plateSpawnAmount > 0)
            {
                Ingredient.SpawnIngredient(IngredientSO, player);
                InteractLogicServerRpc();
            }
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
        plateSpawnAmount--;
        OnPlateRemoved?.Invoke(this, EventArgs.Empty);
    }
}
