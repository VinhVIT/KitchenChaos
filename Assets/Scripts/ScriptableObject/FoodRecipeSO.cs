using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FoodRecipeSO : ScriptableObject
{
    public List<IngredientSO> IngredientSOList;
    public string foodName;
    public int score;
}
