using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitingForPlayerUI : MonoBehaviour
{   
    private void Start()
    {
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerChanged;
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownStart())
        {
            Hide();
        }
    }

    private void GameManager_OnLocalPlayerChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Show();
        }
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
