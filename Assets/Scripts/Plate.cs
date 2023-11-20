using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class Plate : Ingredient
{
    public event EventHandler<OnAddIngredientEventArgs> OnAddIngredient;
    public class OnAddIngredientEventArgs : EventArgs
    {
        public IngredientSO ingredientSO;
    }
    [SerializeField] private List<IngredientSO> validIngredientSOList;
    private List<IngredientSO> IngredientSOList;
    protected override void Awake()
    {   
        base.Awake();  
        
        IngredientSOList = new List<IngredientSO>();
    }
    public bool TryAddIngredient(IngredientSO ingredientSO)
    {
        //Check that ingredient can put on plate or not
        if (!validIngredientSOList.Contains(ingredientSO))
        {
            return false;
        }
        //Check that ingredient is exist on plate or not
        if (IngredientSOList.Contains(ingredientSO))
        {
            return false;
        }
        else
        {
            AddIngredientServerRpc(KitchenGameMultiplayer.Instance.GetIngredientSOIndex(ingredientSO));
            return true;
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void AddIngredientServerRpc(int ingredientSOIndex)
    {
        AddIngredientClientRpc(ingredientSOIndex);
    }
    [ClientRpc]
    private void AddIngredientClientRpc(int ingredientSOIndex)
    {
        IngredientSO ingredientSO = KitchenGameMultiplayer.Instance.GetIngredientSOFromIndex(ingredientSOIndex);
        IngredientSOList.Add(ingredientSO);
            OnAddIngredient?.Invoke(this, new OnAddIngredientEventArgs
            {
                ingredientSO = ingredientSO
            });
    }

    public List<IngredientSO> GetIngredientSOList()
    {
        return IngredientSOList;
    }
}
