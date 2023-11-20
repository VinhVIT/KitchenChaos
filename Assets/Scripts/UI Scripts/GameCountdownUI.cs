using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GameCountdownUI : MonoBehaviour
{   
    private const string countdown = "Countdown";
    [SerializeField] private TextMeshProUGUI countdownText;
    private Animator anim;
    private int previousCountdownNumber;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;
        Hide();

    }
    private void Update()
    {
        int countdownNumber = Mathf.CeilToInt(GameManager.Instance.GetCountdownToStartTimer());
        countdownText.text = countdownNumber.ToString();
        if(previousCountdownNumber != countdownNumber)
        {
            previousCountdownNumber = countdownNumber;
            anim.SetTrigger(countdown);
            SoundManager.Instance.PlayCountdownSound();
        }
    }
    private void GameManager_OnStateChanged(object sender, EventArgs e)
    {
        if (GameManager.Instance.IsCountdownStart())
        {
            Show();
        }
        else
        {
            Hide();
        }
    }
    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
