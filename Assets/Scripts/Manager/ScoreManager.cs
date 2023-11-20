using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : NetworkBehaviour
{   
    public static ScoreManager Instance;
    private int currentScore;

    private void Awake()
    {
        Instance = this;
    }
    [ServerRpc(RequireOwnership = false)]
    public void AddScoreServerRpc(int amount)
    {
        currentScore += amount;
        Debug.Log("added");
    }

    [ServerRpc(RequireOwnership = false)]
    public void MinusScoreServerRpc(int amount)
    {
        currentScore -= amount;
        if (currentScore < 0)
        {
            currentScore = 0;
        }
    }

    public int GetScore()
    {
        return currentScore;
    }
}
