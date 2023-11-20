using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newIngredientData", menuName = "ScriptableObject/Ingredient Data", order = 0)]
public class IngredientSO : ScriptableObject
{
    public Transform prefab;
    public Sprite iconSprite;
    public string objectName;
}
