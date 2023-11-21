using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenGameMultiplayer : NetworkBehaviour
{
    private const int MAX_PLAYER_AMOUNT = 4;
    public static KitchenGameMultiplayer Instance { get; private set; }
    public event EventHandler OnTryingToJoinGame;
    public event EventHandler OnFailedToJoinGame;

    [SerializeField] private IngredientListSO ingredientListSO;


    private void Awake()
    {
        Instance = this;

        // DontDestroyOnLoad(gameObject);
    }
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {   
        if(SceneManager.GetActiveScene().name != Loader.Scene.CharacterSelectScene.ToString())
        {
            response.Approved = false;
            response.Reason = "Game has already started";
            return;
        }
        if(NetworkManager.Singleton.ConnectedClientsIds.Count >= MAX_PLAYER_AMOUNT){
            response.Approved = false;
            response.Reason = "Game is full";
            return;
        }

        response.Approved = true;
    }

    public void StartClient()
    {   
        OnTryingToJoinGame?.Invoke(this,EventArgs.Empty);
        NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
        NetworkManager.Singleton.StartClient();
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong clientId)
    {
        OnFailedToJoinGame?.Invoke(this,EventArgs.Empty);
    }

    public void SpawnIngredient(IngredientSO ingredientSO, IIngredientParent ingredientParent)
    {
        SpawnIngredientServerRpc(GetIngredientSOIndex(ingredientSO), ingredientParent.GetNetworkObject());
    }

    [ServerRpc(RequireOwnership = false)]
    private void SpawnIngredientServerRpc(int ingredientSOIndex, NetworkObjectReference ingredientParentNetworkObjectReference)
    {
        IngredientSO ingredientSO = GetIngredientSOFromIndex(ingredientSOIndex);

        Transform ingredientTransform = Instantiate(ingredientSO.prefab);

        NetworkObject ingredientNetworkObject = ingredientTransform.GetComponent<NetworkObject>();
        ingredientNetworkObject.Spawn(true);

        Ingredient ingredient = ingredientTransform.GetComponent<Ingredient>();

        ingredientParentNetworkObjectReference.TryGet(out NetworkObject ingredientParentNetworkObject);
        IIngredientParent ingredientParent = ingredientParentNetworkObject.GetComponent<IIngredientParent>();

        ingredient.SetIngredientParent(ingredientParent);
    }

    public int GetIngredientSOIndex(IngredientSO ingredientSO)
    {
        return ingredientListSO.ingredientSOList.IndexOf(ingredientSO);
    }

    public IngredientSO GetIngredientSOFromIndex(int ingredientSOIndex)
    {
        return ingredientListSO.ingredientSOList[ingredientSOIndex];
    }

    public void DestroyIngredient(Ingredient ingredient)
    {
        DestroyIngredientServerRpc(ingredient.NetworkObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DestroyIngredientServerRpc(NetworkObjectReference ingredientParentNetworkObjectReference)
    {
        ingredientParentNetworkObjectReference.TryGet(out NetworkObject ingredientNetworkObject);
        Ingredient ingredient = ingredientNetworkObject.GetComponent<Ingredient>();

        ClearIngredientOnParentClientRpc(ingredientParentNetworkObjectReference);
        ingredient.DestroySelf();
    }
    [ClientRpc]
    private void ClearIngredientOnParentClientRpc(NetworkObjectReference ingredientParentNetworkObjectReference)
    {
        ingredientParentNetworkObjectReference.TryGet(out NetworkObject ingredientNetworkObject);
        Ingredient ingredient = ingredientNetworkObject.GetComponent<Ingredient>();

        ingredient.ClearIngredientOnParent();
    }
}
