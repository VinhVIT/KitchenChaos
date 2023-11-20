using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateCompleteVisual : MonoBehaviour
{
    [Serializable]
    public struct IngredientSO_GameObject
    {
        public IngredientSO IngredientSO;
        public GameObject gameObject;
    }
    [SerializeField] private Plate plate;
    [SerializeField] private List<IngredientSO_GameObject> IngredientSOGameObjectList;
    private void Start()
    {
        plate.OnAddIngredient += Plate_OnAddIngredient;
        foreach (IngredientSO_GameObject IngredientSOGameObject in IngredientSOGameObjectList)
        {
            IngredientSOGameObject.gameObject.SetActive(false);
        }
    }   

    private void Plate_OnAddIngredient(object sender,Plate.OnAddIngredientEventArgs  e)
    {
        foreach (IngredientSO_GameObject IngredientSOGameObject in IngredientSOGameObjectList)
        {
            //check ingredient receive from game match with ingredient in the list or not
            if (IngredientSOGameObject.IngredientSO == e.ingredientSO)
            {
                IngredientSOGameObject.gameObject.SetActive(true);
            }
        }
    }
}
