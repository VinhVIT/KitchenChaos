using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DeliveryManagerUI : MonoBehaviour
{
    private const string OVER_TIME = "overTime";
    [SerializeField] private Transform container;
    [SerializeField] private Transform recipeTemplate;
    [SerializeField] private TextMeshProUGUI scoreText;
    private Animator recipeAnim;
    private void Awake()
    {
        recipeTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSpawned += DeliveryManagerUI_OnRecipeSpawned;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManagerUI_OnRecipeCompleted;
        DeliveryManager.Instance.OnRecipeOverTime += DeliveryManager_onRecipeOverTime;
        UpdateVisual();
    }

    private void DeliveryManager_onRecipeOverTime(object sender, EventArgs e)
    {   
        recipeAnim = container.GetChild(1).GetComponent<Animator>();
        recipeAnim.SetBool(OVER_TIME, true);
    }
    private void DeliveryManagerUI_OnRecipeCompleted(object sender, EventArgs e)
    {
        UpdateVisual();
        scoreText.text = ScoreManager.Instance.GetScore().ToString();
    }

    private void DeliveryManagerUI_OnRecipeSpawned(object sender, EventArgs e)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        foreach (Transform child in container)
        {
            if (child == recipeTemplate) continue;
            Destroy(child.gameObject);
        }
        foreach (FoodRecipeSO recipeSO in DeliveryManager.Instance.GetwaitingFoodSOList())
        {
            //Spawn Order
            Transform recipeTransform = Instantiate(recipeTemplate, container);
            recipeTransform.gameObject.SetActive(true);
            //Set Order Property
            recipeTransform.GetComponent<DeliveryManagerSingleUI>().SetRecipeSO(recipeSO);
        }
    }
}
