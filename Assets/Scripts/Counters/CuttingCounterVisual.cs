using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounterVisual : MonoBehaviour
{
    private const string Cut = "Cut";
    private Animator anim;
    [SerializeField] private CuttingCounter cuttingCounter;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        cuttingCounter.OnCuttingAnim += CuttingCounter_OnCuttingAnim;
    }

    private void CuttingCounter_OnCuttingAnim(object sender, EventArgs e)
    {
        anim.SetTrigger(Cut);
    }
}
