using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePauseUI : MonoBehaviour
{   
    private void Start()
    {
        GameManager.Instance.OnLocalGamePaused += GameManager_OnLocalGamePaused;
        GameManager.Instance.OnLocalGameUnPaused += GameManager_OnLocalGameUnPaused;

        Hide();
    }

    private void GameManager_OnLocalGameUnPaused(object sender, EventArgs e)
    {
        Hide();
    }

    private void GameManager_OnLocalGamePaused(object sender, EventArgs e)
    {
        Show();
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
