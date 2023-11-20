using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePlayingClockUI : MonoBehaviour
{
    [SerializeField] private Image TimerClock;
    private void Update()
    {
        TimerClock.fillAmount = GameManager.Instance.GetGamePlayingTimerNormalized();
    }
}
