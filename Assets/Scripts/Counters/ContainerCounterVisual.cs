using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContainerCounterVisual : MonoBehaviour
{
    private const string OPEN_CLOSE = "OpenClose";
    private Animator anim;
    [SerializeField] private ContainerCounter containerCounter;
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }
    private void Start()
    {
        containerCounter.OnSpawnIngredient += ContainerCounter_OnSpawnIngredient;
    }
    private void ContainerCounter_OnSpawnIngredient(object sender, EventArgs e)
    {
        anim.SetTrigger(OPEN_CLOSE);
    }

}
