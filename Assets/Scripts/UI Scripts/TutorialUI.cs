using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{

    private void Start()
    {
        Show();
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        GameManager.Instance.OnLocalPlayerReadyChanged += GameManager_OnLocalPlayerChanged;
    }

    private void GameManager_OnLocalPlayerChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsLocalPlayerReady())
        {
            Hide();
        }
    }

    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownStart())
        {
            Hide();
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
