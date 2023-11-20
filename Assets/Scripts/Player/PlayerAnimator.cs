using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerAnimator : NetworkBehaviour
{
    private const string IS_WALKING = "IsWalking";
    private const string IS_HOLDING = "IsHolding";
    [SerializeField] private Player player;
    private Animator anim;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }
    private void Update()
    {
        if (!IsOwner) return;
        if (!GameManager.Instance.IsGamePaused())
        {
            anim.SetBool(IS_WALKING, player.CheckWalking());
            anim.SetBool(IS_HOLDING, player.HasIngredient());
        }
    }
}
