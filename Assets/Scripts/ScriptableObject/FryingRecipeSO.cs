using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class FryingRecipeSO : ScriptableObject
{
    public IngredientSO input;
    public IngredientSO output;
    public float maxFryingTimer;
}
