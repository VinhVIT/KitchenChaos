using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class Ingredient : NetworkBehaviour
{
    [SerializeField] private IngredientSO IngredientSO;
    private IIngredientParent ingredientParent;
    private FollowTransform followTransform;
    protected virtual void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }
    public IngredientSO GetIngredientSO()
    {
        return IngredientSO;
    }
    public void SetIngredientParent(IIngredientParent ingredientParent)
    {
        SetIngredientParentServerRpc(ingredientParent.GetNetworkObject());
    }
    public void DestroySelf()
    {
        ingredientParent.ClearIngredient();
        Destroy(gameObject);
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetIngredientParentServerRpc(NetworkObjectReference IngredientParentNetworkObjectReference)
    {
        SetIngredientParentClientRpc(IngredientParentNetworkObjectReference);
    }
    [ClientRpc]
    private void SetIngredientParentClientRpc(NetworkObjectReference IngredientParentNetworkObjectReference)
    {
        IngredientParentNetworkObjectReference.TryGet(out NetworkObject IngredientParentNetworkObject);
        IIngredientParent ingredientParent = IngredientParentNetworkObject.GetComponent<IIngredientParent>();

        if (this.ingredientParent != null)
        {
            this.ingredientParent.ClearIngredient();
        }

        this.ingredientParent = ingredientParent;
        if (ingredientParent.HasIngredient()) Debug.LogError("This counter already has ingredient on it !");

        ingredientParent.SetIngredient(this);

        followTransform.SetTargetTransform(ingredientParent.GetIngredientFollowTransform());
    }
    public IIngredientParent GetIngredientParent() => ingredientParent;
    public void ClearIngredientOnParent()
    {
        ingredientParent.ClearIngredient();
    }
    public bool TryGetPlate(out Plate plate)
    {
        if (this is Plate)
        {
            plate = this as Plate;
            return true;
        }
        else
        {
            plate = null;
            return false;
        }
    }

    public static void SpawnIngredient(IngredientSO IngredientSO, IIngredientParent ingredientParent)
    {
        KitchenGameMultiplayer.Instance.SpawnIngredient(IngredientSO, ingredientParent);
    }
    public static void DestroyIngredient(Ingredient ingredient)
    {
        KitchenGameMultiplayer.Instance.DestroyIngredient(ingredient);
    }
}
