using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSound : MonoBehaviour
{
    private Player player;
    private float footstepTimer;
    private float maxFootstepTimer = .1f;
    private void Awake()
    {
        player = GetComponent<Player>();
    }
    private void Update()
    {
        footstepTimer += maxFootstepTimer;
        if (footstepTimer > maxFootstepTimer)
        {
            footstepTimer = 0;
            if (player.CheckWalking())
            {   
                float volume = 1f;
                SoundManager.Instance.PlayFootstepSound(player.transform.position,volume);
            }
        }
    }
}
