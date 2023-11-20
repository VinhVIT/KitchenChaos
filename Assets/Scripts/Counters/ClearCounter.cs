using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : BaseCounter
{
    public override void Interact(Player player)
    {
        if (!HasIngredient())
        //there no ingredient on table
        {
            if (player.HasIngredient())
            {
                //put ingredient on the table 
                player.GetIngredient().SetIngredientParent(this);
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
                else
                //Player not carry plate
                {
                    if (GetIngredient().TryGetPlate(out plate))
                    //There a plate on this counter
                    {
                        if (plate.TryAddIngredient(player.GetIngredient().GetIngredientSO()))
                        {
                            Ingredient.DestroyIngredient(player.GetIngredient());
                        }
                    }
                }
            }
            else
            {
                GetIngredient().SetIngredientParent(player);
            }

        }
    }

}
