using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProgressBarWarningUI : MonoBehaviour
{
    private const string IS_FLASHING = "isFlashing";
    [SerializeField] StoveCounter stoveCounter;
    [SerializeField] private Animator flashingAnim;
    private float warningSoundTimer;

    private void Start()
    {
        stoveCounter.OnStateChanged += StoveCounter_OnStateChanged;
        Hide();
    }

    private void StoveCounter_OnStateChanged(object sender, StoveCounter.OnStateChangedEventArgs e)
    {
        if (stoveCounter.isFried())
        {
            Show();
            flashingAnim.SetBool(IS_FLASHING, true);
            SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
        }
        else
        {
            Hide();
            flashingAnim.SetBool(IS_FLASHING, false);
        }
    }
    private void Update()
    {
        if (stoveCounter.isFried())
        {   
            float maxWarningSoundTimer = .2f;
            warningSoundTimer += Time.deltaTime;
            if(warningSoundTimer > maxWarningSoundTimer){
                warningSoundTimer = 0;
                SoundManager.Instance.PlayWarningSound(stoveCounter.transform.position);
            }   
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
