using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipeSO : ScriptableObject
{
    public IngredientSO input;
    public IngredientSO output;
    public int maxCuttingProgess;
}
