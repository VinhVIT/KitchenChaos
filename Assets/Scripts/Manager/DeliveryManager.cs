using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;

public class DeliveryManager : NetworkBehaviour
{
    public static DeliveryManager Instance { get; private set; }

    public event EventHandler OnRecipeSpawned;
    public event EventHandler OnRecipeCompleted;
    public event EventHandler OnRecipeSuccess;
    public event EventHandler OnRecipeFailed;
    public event EventHandler OnRecipeOverTime;
    [SerializeField] private FoodListSO FoodListSO;
    private List<FoodRecipeSO> waitingFoodSOList;
    private int maxWaitingRecipe = 4;
    private float spawnRecipeTimer = 4f;
    private float maxSpawnReipeTimer = 5f;
    private float currentWaitingTimer;
    private float maxWaitingTimer = 20f;
    private int scorePoint = 0;

    private void Awake()
    {
        Instance = this;
        waitingFoodSOList = new List<FoodRecipeSO>();
    }
    private void Update()
    {
        if (!IsServer) return;
        SpawnWaitingRecipe();
        RemoveOverTimeRecipe();
    }
    private void SpawnWaitingRecipe()
    {
        spawnRecipeTimer -= Time.deltaTime;
        if (spawnRecipeTimer <= 0f)
        {
            spawnRecipeTimer = maxSpawnReipeTimer;
            if (GameManager.Instance.IsPlaying() && waitingFoodSOList.Count < maxWaitingRecipe)
            //make food request
            {
                int waitingFoodSOIndex = UnityEngine.Random.Range(0, FoodListSO.recipeSOList.Count);

                SpawnWaitingRecipeClientRpc(waitingFoodSOIndex);
            }
        }
    }
    [ClientRpc]
    private void SpawnWaitingRecipeClientRpc(int waitingFoodSOIndex)
    {
        FoodRecipeSO waitingRecipeSO = FoodListSO.recipeSOList[waitingFoodSOIndex];
        waitingFoodSOList.Add(waitingRecipeSO);
        OnRecipeSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void RemoveOverTimeRecipe()
    {
        float runningOutOfTimer = 30f;
        if (waitingFoodSOList.Count > 0)
        {
            currentWaitingTimer += Time.deltaTime;
            if (currentWaitingTimer > runningOutOfTimer)
            {
                OnRecipeOverTime?.Invoke(this, EventArgs.Empty);
                if (currentWaitingTimer > maxWaitingTimer)
                {
                    RemoveOverTimeRecipeClientRpc();
                }
            }

        }
    }
    [ClientRpc]
    private void RemoveOverTimeRecipeClientRpc()
    {
        currentWaitingTimer = 0f;

        scorePoint -= 100;
        waitingFoodSOList.RemoveAt(0);

        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
    }
    public void DeliveryRecipe(Plate plate)
    {
        for (int i = 0; i < waitingFoodSOList.Count; i++)
        {
            FoodRecipeSO waitingRecipeSO = waitingFoodSOList[i];

            if (waitingRecipeSO.IngredientSOList.Count == plate.GetIngredientSOList().Count)
            //Has the same number of ingredient
            {
                bool plateContentsMatchesRecipe = true;
                foreach (IngredientSO recipeIngredientSO in waitingRecipeSO.IngredientSOList)
                // Cycling through all ingredient in the recipe 
                {
                    bool ingredientFound = false;
                    foreach (IngredientSO plateIngredientSO in plate.GetIngredientSOList())
                    // Cycling through all ingredient in the plate 
                    {
                        if (plateIngredientSO == recipeIngredientSO)
                        //ingredient matches!!!
                        {
                            ingredientFound = true;
                            break;
                        }
                    }
                    if (!ingredientFound)
                    //Can found recipe ingredient on the plate
                    {
                        plateContentsMatchesRecipe = false;
                    }
                }
                if (plateContentsMatchesRecipe)
                //Player delivery the correct recipe!!!
                {
                    DeliveryCorrectRecipeServerRpc(i);

                    return;
                }
            }
        }
        //player didnot delivery a correct recipe:(
        DeliveryIncorrectRecipeServerRpc();
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryIncorrectRecipeServerRpc()
    {
        DeliveryIncorrectRecipeClientRpc();
    }
    [ClientRpc]
    private void DeliveryIncorrectRecipeClientRpc()
    {
        OnRecipeFailed?.Invoke(this, EventArgs.Empty);
    }
    [ServerRpc(RequireOwnership = false)]
    private void DeliveryCorrectRecipeServerRpc(int waitingFoodSOListIndex)
    {
        DeliveryCorrectRecipeClientRpc(waitingFoodSOListIndex);
    }
    [ClientRpc]
    private void DeliveryCorrectRecipeClientRpc(int waitingFoodSOListIndex)
    {
        if (waitingFoodSOListIndex == 0)
        {
            currentWaitingTimer = 0f;
        }

        scorePoint += waitingFoodSOList[waitingFoodSOListIndex].score;
        waitingFoodSOList.RemoveAt(waitingFoodSOListIndex);


        OnRecipeCompleted?.Invoke(this, EventArgs.Empty);
        OnRecipeSuccess?.Invoke(this, EventArgs.Empty);
    }
    public List<FoodRecipeSO> GetwaitingFoodSOList() => waitingFoodSOList;
    public int GetScorePoint()
    {
        return scorePoint;
    }
}
