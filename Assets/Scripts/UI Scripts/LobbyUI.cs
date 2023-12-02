using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUI : MonoBehaviour
{
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Button createLobbyButton;
    [SerializeField] private Button quickJoinButton;
    [SerializeField] private Button joinCodeButton;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyListContainer;
    [SerializeField] private Transform lobbyTemplate;


    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenuScene);
            GameLobby.Instance.LeaveLobby();
        });
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
        });
        quickJoinButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.QuickJoin();
        });
        joinCodeButton.onClick.AddListener(() =>
        {
            GameLobby.Instance.JoinWithCode(codeInputField.text);
        });
        lobbyTemplate.gameObject.SetActive(false);
    }
    private void Start()
    {
        playerNameInputField.text = KitchenGameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            KitchenGameMultiplayer.Instance.SetPlayerName(newText);
        });

        GameLobby.Instance.OnLobbyListChanged += GameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void GameLobby_OnLobbyListChanged(object sender, GameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e.lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {   
        foreach (Transform child in lobbyListContainer)
        {
            if (child == lobbyTemplate) {
                continue;
            }
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {   
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyListContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }
}
