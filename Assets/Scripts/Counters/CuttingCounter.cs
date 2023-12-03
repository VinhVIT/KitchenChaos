using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public static event EventHandler OnAnyCut;
    new public static void ResetStaticData() { OnAnyCut = null; }
    public event EventHandler<IHasProgress.OnProgressChangedEventArg> OnProgressChanged;
    public event EventHandler OnCuttingAnim;
    private int cuttingProgress;
    [SerializeField] private ProgressBarUI progressBarUI;
    [SerializeField] private CuttingRecipeSO[] cutingRecipeSOArray;
    public override void Interact(Player player)
    {
        if (!HasIngredient())
        //there no ingredient on table
        {
            if (player.HasIngredient())
            {
                //Check if that ingredient can cut or not 
                if (HasRecipeInput(player.GetIngredient().GetIngredientSO()))
                {
                    //put ingredient on the table 
                    Ingredient ingredient = player.GetIngredient();
                    ingredient.SetIngredientParent(this);

                    InteractLogicPlaceObjectOnCounterServerRpc();
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
                    }
                }
            }
            else
            {
                GetIngredient().SetIngredientParent(player);
                progressBarUI.Hide();
            }

        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void InteractLogicPlaceObjectOnCounterServerRpc()
    {
        InteractLogicPlaceObjectOnCounterClientRpc();
    }
    [ClientRpc]
    private void InteractLogicPlaceObjectOnCounterClientRpc()
    {
        cuttingProgress = 0;

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArg
        {
            currentProgess = 0f
        });
    }
    public override void Interact2(Player player)
    {
        if (HasIngredient() && HasRecipeInput(GetIngredient().GetIngredientSO()))
        //Has Ingredient here and can cut it 
        {
            CutObjectServerRpc();
            TestCuttingProgressDoneServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void CutObjectServerRpc()
    {
        if (HasIngredient() && HasRecipeInput(GetIngredient().GetIngredientSO()))
        //Has Ingredient here and can cut it 
        {
            CutObjectClientRpc();
        }
    }
    [ClientRpc]
    private void CutObjectClientRpc()
    {
        cuttingProgress++;

        OnCuttingAnim?.Invoke(this, EventArgs.Empty);
        OnAnyCut?.Invoke(this, EventArgs.Empty);

        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetIngredient().GetIngredientSO());

        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArg
        {
            currentProgess = (float)cuttingProgress / cuttingRecipeSO.maxCuttingProgess
        });
    }
    [ServerRpc(RequireOwnership = false)]
    private void TestCuttingProgressDoneServerRpc()
    {
        if (HasIngredient() && HasRecipeInput(GetIngredient().GetIngredientSO()))
        {
            CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetIngredient().GetIngredientSO());
            if (cuttingProgress >= cuttingRecipeSO.maxCuttingProgess)
            {
                IngredientSO outputIngredientSO = GetOutPutForInput(GetIngredient().GetIngredientSO());
                Ingredient.DestroyIngredient(GetIngredient());

                Ingredient.SpawnIngredient(outputIngredientSO, this);
            }
        }
    }
    private bool HasRecipeInput(IngredientSO inputIngredientSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputIngredientSO);
        return cuttingRecipeSO != null;
    }
    private IngredientSO GetOutPutForInput(IngredientSO inputIngredientSO)
    {
        CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputIngredientSO);
        if (cuttingRecipeSO != null)
        {
            return cuttingRecipeSO.output;
        }
        else
        {
            return null;
        }

    }
    private CuttingRecipeSO GetCuttingRecipeSOWithInput(IngredientSO inputIngredientSO)
    {
        foreach (CuttingRecipeSO cuttingRecipeSO in cutingRecipeSOArray)
        {
            if (cuttingRecipeSO.input == inputIngredientSO)
            {
                return cuttingRecipeSO;
            }
        }
        return null;
    }
}
