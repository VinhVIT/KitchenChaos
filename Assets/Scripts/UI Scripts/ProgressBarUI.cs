using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{   
    [SerializeField] private Image barImage;
    [SerializeField] private GameObject hasProgessGameObject;
    private IHasProgress hasProgress;
    private void Start()
    {
        hasProgress = hasProgessGameObject.GetComponent<IHasProgress>();
        if (hasProgress == null) Debug.LogError("Game Object " + hasProgessGameObject + " doesnt have component that implement IHasProgess!");
        hasProgress.OnProgressChanged += ProgressBar_OnProgressChanged;

        barImage.fillAmount = 0f;
        Hide();
    }

    private void ProgressBar_OnProgressChanged(object sender, IHasProgress.OnProgressChangedEventArg e)
    {
        barImage.fillAmount = e.currentProgess;
        if (barImage.fillAmount <= 0 || barImage.fillAmount >= 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }
    private void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
