using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryCounter : BaseCounter
{
    public static DeliveryCounter Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }
    public override void Interact(Player player)
    {
        if (player.HasIngredient())
        {
            if (player.GetIngredient().TryGetPlate(out Plate plate))
            //check if there a plate is delivery so destroy it
            {
                DeliveryManager.Instance.DeliveryRecipe(plate);
                Ingredient.DestroyIngredient(player.GetIngredient());
            }
        }
    }
}
