using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class BurningRecipeSO : ScriptableObject
{
    public IngredientSO input;
    public IngredientSO output;
    public float maxBurningTimer;
}
