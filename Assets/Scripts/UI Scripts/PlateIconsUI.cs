using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateIconsUI : MonoBehaviour
{
    [SerializeField] private Plate plate;
    [SerializeField] private Transform iconTemplate;
    private void Awake()
    {
        iconTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        plate.OnAddIngredient += PlateIconsUI_OnAddIngredient;
    }

    private void PlateIconsUI_OnAddIngredient(object sender, Plate.OnAddIngredientEventArgs e)
    {
        UpdateVisual();
    }
    private void UpdateVisual()
    {
        foreach (Transform child in transform)
        {
            if (child != iconTemplate)
            {
                Destroy(child.gameObject);
            }
        }
        foreach (IngredientSO IngredientSO in plate.GetIngredientSOList())
        {
            Transform iconTransform = Instantiate(iconTemplate, transform);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<PlateIconSingleUI>().SetIngredientSO(IngredientSO);
        }
    }
}
