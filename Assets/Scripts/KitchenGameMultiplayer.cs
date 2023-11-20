using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class KitchenGameMultiplayer : NetworkBehaviour
{

    public static KitchenGameMultiplayer Instance { get; private set; }

    [SerializeField] private IngredientListSO ingredientListSO;


    private void Awake()
    {
        Instance = this;

        DontDestroyOnLoad(gameObject);
    }
    public void StartHost()
    {
        NetworkManager.Singleton.ConnectionApprovalCallback += NetworkManager_ConnectionApprovalCallback;
        NetworkManager.Singleton.StartHost();
    }

    private void NetworkManager_ConnectionApprovalCallback(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
    {   
        response.Approved = true;
        // if (GameManager.Instance.IsWaitingToStart())
        // {
        //     response.Approved = true;
        //     response.CreatePlayerObject = true;
        // }
        // else
        // {
        //     response.Approved = false;
        // }
    }

    public void StartClient()
    {
        NetworkManager.Singleton.StartClient();
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
