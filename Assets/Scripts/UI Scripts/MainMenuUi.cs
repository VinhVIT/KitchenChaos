using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuUi : MonoBehaviour
{
    [SerializeField] private Button multiplayerButton;
    [SerializeField] private Button singleplayerButton;

    [SerializeField] private Button quitButton;
    private void Awake() {
        multiplayerButton.onClick.AddListener(() =>{
            KitchenGameMultiplayer.playMultiplayer = true;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        singleplayerButton.onClick.AddListener(() =>{
            KitchenGameMultiplayer.playMultiplayer = false;
            Loader.Load(Loader.Scene.LobbyScene);
        });
        quitButton.onClick.AddListener(() =>{
            Application.Quit();
        });
        Time.timeScale = 1f;
    }
}
