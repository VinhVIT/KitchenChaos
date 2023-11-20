using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class DeliveryResultUI : MonoBehaviour
{   
    private const string POP_UP = "popUp";
    [SerializeField] private Image backgroundImg;
    [SerializeField] private Image iconImg;
    [SerializeField] private TextMeshProUGUI message;
    [SerializeField] private Color successColor;
    [SerializeField] private Color failedColor;
    [SerializeField] private Sprite successIcon;
    [SerializeField] private Sprite faiedIcon;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryManager_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryManager_OnRecipeFailed;
        Hide();
    }

    private void DeliveryManager_OnRecipeFailed(object sender, EventArgs e)
    {   
        Show();
        backgroundImg.color = failedColor;
        iconImg.sprite = faiedIcon;
        message.text = "DELIVERY\nFAILED";
        anim.SetTrigger(POP_UP);
    }

    private void DeliveryManager_OnRecipeSuccess(object sender, EventArgs e)
    {
        Show();
        backgroundImg.color = successColor;
        iconImg.sprite = successIcon;
        message.text = "DELIVERY\nSUCCESS";
        anim.SetTrigger(POP_UP);
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
