using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{   
    public static ScoreManager Instance;
    private int currentScore;

    private void Awake()
    {
        Instance = this;
    }
    public void AddScore(int amount)
    {
        currentScore += amount;
        Debug.Log("added");
    }

    public void MinusScore(int amount)
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
