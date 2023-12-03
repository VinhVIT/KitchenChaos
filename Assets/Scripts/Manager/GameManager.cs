using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameManager : NetworkBehaviour
{

    public static GameManager Instance { get; private set; }
    public event EventHandler OnStateChanged;
    public event EventHandler OnLocalGamePaused;
    public event EventHandler OnLocalGameUnPaused;
    public event EventHandler OnMultiplayerGamePaused;
    public event EventHandler OnMultiplayerGameUnPaused;
    public event EventHandler OnLocalPlayerReadyChanged;
    public enum State
    {
        waitingToStart,
        countdownToStart,
        gamePlaying,
        gameOver,
    }
    [SerializeField] private Transform playerPrefab;
    private NetworkVariable<State> state = new NetworkVariable<State>(State.waitingToStart);
    private bool isLocalPlayerReady;
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(0f);
    private float maxGamePlayingTimer = 100f;
    private bool isLocalGamePaused = false;
    private bool autoTestGamePaused;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(false);
    private Dictionary<ulong, bool> playerReadyDictionary;
    private Dictionary<ulong, bool> playerPausedDictionary;
    private void Awake()
    {
        Instance = this;

        playerReadyDictionary = new Dictionary<ulong, bool>();
        playerPausedDictionary = new Dictionary<ulong, bool>();
    }
    private void Start()
    {
        GameInput.Instance.OnPauseGame += GameInput_OnPauseGame;
        GameInput.Instance.OnInteractAction += GameInput_OnInteractAction;
    }
    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        isGamePaused.OnValueChanged += IsGamePaused_OnValueChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += NetworkManager_OnClientDisconnectCallback;
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform playerTransform = Instantiate(playerPrefab);
            playerTransform.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void NetworkManager_OnClientDisconnectCallback(ulong obj)
    {
        autoTestGamePaused = true;
    }

    private void IsGamePaused_OnValueChanged(bool previousValue, bool newValue)
    {
        if (isGamePaused.Value)
        {
            Time.timeScale = 0f;
            OnMultiplayerGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            Time.timeScale = 1f;
            OnMultiplayerGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }

    private void State_OnValueChanged(State previousValue, State newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void GameInput_OnInteractAction(object sender, EventArgs e)
    {
        if (state.Value == State.waitingToStart)
        {
            isLocalPlayerReady = true;

            OnLocalPlayerReadyChanged?.Invoke(this, EventArgs.Empty);

            SetPlayerReadyServerRpc();
        }
    }
    [ServerRpc(RequireOwnership = false)]
    private void SetPlayerReadyServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerReadyDictionary[serverRpcParams.Receive.SenderClientId] = true;

        bool allClientId = true;
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (!playerReadyDictionary.ContainsKey(clientId) || !playerReadyDictionary[clientId])
            {
                //this player is not ready
                allClientId = false;
                break;
            }
        }
        if (allClientId)
        {
            state.Value = State.countdownToStart;
        }
    }
    private void GameInput_OnPauseGame(object sender, EventArgs e)
    {
        PauseGame();
    }

    private void Update()
    {
        if (!IsServer) return;
        switch (state.Value)
        {
            case State.waitingToStart:

                break;
            case State.countdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = State.gamePlaying;
                    gamePlayingTimer.Value = maxGamePlayingTimer;
                    OnStateChanged?.Invoke(this, EventArgs.Empty);
                }
                break;
            case State.gamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = State.gameOver;
                }
                break;
            case State.gameOver:
                break;
        }
    }
    private void LateUpdate()
    {
        if (autoTestGamePaused)
        {
            autoTestGamePaused = false;
            TestGamePaused();
        }
    }
    public bool IsPlaying() => state.Value == State.gamePlaying;
    public bool IsCountdownStart() => state.Value == State.countdownToStart;
    public bool IsGameOver() => state.Value == State.gameOver;

    public bool IsLocalPlayerReady() => isLocalPlayerReady;
    public bool IsWaitingToStart() => state.Value == State.waitingToStart;
    public float GetCountdownToStartTimer() => countdownToStartTimer.Value;
    public float GetGamePlayingTimerNormalized() => (gamePlayingTimer.Value / maxGamePlayingTimer);
    public void PauseGame()
    {
        isLocalGamePaused = !isLocalGamePaused;
        if (isLocalGamePaused)
        {
            PauseGameServerRpc();
            OnLocalGamePaused?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            UnpauseGameServerRpc();
            OnLocalGameUnPaused?.Invoke(this, EventArgs.Empty);
        }
    }
    public void ReturnMainMenu()
    {
        NetworkManager.Singleton.Shutdown();
        Loader.Load(Loader.Scene.MainMenuScene);
    }
    public void RetryGame()
    {
        Loader.Load(Loader.Scene.GameScene);
    }
    [ServerRpc(RequireOwnership = false)]
    private void PauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = true;
        TestGamePaused();
    }
    [ServerRpc(RequireOwnership = false)]
    private void UnpauseGameServerRpc(ServerRpcParams serverRpcParams = default)
    {
        playerPausedDictionary[serverRpcParams.Receive.SenderClientId] = false;
        TestGamePaused();
    }
    private void TestGamePaused()
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            if (playerPausedDictionary.ContainsKey(clientId) && playerPausedDictionary[clientId])
            {
                //this player had paused the game
                isGamePaused.Value = true;
                return;
            }
        }
        //all players are unpaused
        isGamePaused.Value = false;
    }
}
