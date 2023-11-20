using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{   
    private const string PLAYER_PREFS_SOUND_EFFECT_VOLUME = "SoundEffectsVolume";
    public static SoundManager Instance;
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;
    private float volumeMultiply = 0.5f;

    private void Awake()
    {
        Instance = this;

        volumeMultiply = PlayerPrefs.GetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, 0.5f);
    }
    private void Start()
    {
        DeliveryManager.Instance.OnRecipeSuccess += DeliveryCounter_OnRecipeSuccess;
        DeliveryManager.Instance.OnRecipeFailed += DeliveryCounter_OnRecipeFailed;
        CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        Player.OnAnyPicking += Player_OnPickedSomething;
        BaseCounter.OnSomethingPlacedHere += BaseCounter_OnSomethingPlacedHere;
        TrashCounter.OnReceiveTrash += TrashCounter_OnReceiveTrash;
    }

    private void TrashCounter_OnReceiveTrash(object sender, EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }

    private void BaseCounter_OnSomethingPlacedHere(object sender, EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.objectDrop, baseCounter.transform.position);
    }

    private void Player_OnPickedSomething(object sender, EventArgs e)
    {   
        Player player = sender as Player;
        PlaySound(audioClipRefsSO.objectPickup, player.transform.position);
    }

    private void CuttingCounter_OnAnyCut(object sender, EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chop, cuttingCounter.transform.position);
    }

    private void DeliveryCounter_OnRecipeFailed(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliveryFailed, DeliveryCounter.Instance.transform.position);
    }

    private void DeliveryCounter_OnRecipeSuccess(object sender, EventArgs e)
    {
        PlaySound(audioClipRefsSO.deliverySuccess, DeliveryCounter.Instance.transform.position);
    }

    private void PlaySound(AudioClip audioClip, Vector3 position, float volume = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, volume * volumeMultiply);
    }
    private void PlaySound(AudioClip[] audioClipArray, Vector3 position, float volume = 1f)
    {
        PlaySound(audioClipArray[UnityEngine.Random.Range(0, audioClipArray.Length)], position, volume * volumeMultiply);
    }
    public void PlayFootstepSound(Vector3 position, float volume)
    {
        PlaySound(audioClipRefsSO.footstep, position, volume);
    }
    public void PlayCountdownSound()
    {
        PlaySound(audioClipRefsSO.warning, Vector3.zero);
    }
    public void PlayWarningSound(Vector3 position)
    {
        PlaySound(audioClipRefsSO.warning, position);
    }
    public void ChangeVolume(float value)
    {
        volumeMultiply = value;

        PlayerPrefs.SetFloat(PLAYER_PREFS_SOUND_EFFECT_VOLUME, volumeMultiply);
        PlayerPrefs.Save();
    }
    public float GetVolumeMultiply()
    {
        return volumeMultiply;
    }
}
