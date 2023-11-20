using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{   
    private const string PLAYER_PREFS_MUSIC_VOLUME = "MusicVolume";
    public static MusicManager Instance;
    private AudioSource audioSource;
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();

        audioSource.volume = PlayerPrefs.GetFloat(PLAYER_PREFS_MUSIC_VOLUME, 0.5f);
    }
    public void ChangeMusicVolume(float value)
    {
        audioSource.volume = value;
        PlayerPrefs.SetFloat(PLAYER_PREFS_MUSIC_VOLUME, audioSource.volume);
        PlayerPrefs.Save();
    }
    public float GetMusicVolume()
    {
        return audioSource.volume;
    }
}
