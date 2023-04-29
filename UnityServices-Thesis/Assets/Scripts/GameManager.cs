using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance { get; private set; }

    public event EventHandler OnStateChanged;
    
    private enum GameState
    {
        WaitingToStart,
        CountdownToStart,
        GamePlaying,
        GameOver
    }

    [SerializeField] private Transform playerPrefab;

    private NetworkVariable<GameState> state = new NetworkVariable<GameState>(GameState.WaitingToStart);
    private NetworkVariable<float> waitingToStartTimer = new NetworkVariable<float>(.1f);
    private NetworkVariable<float> countdownToStartTimer = new NetworkVariable<float>(3f);
    private NetworkVariable<float> gamePlayingTimer = new NetworkVariable<float>(100f);

    private void Awake()
    {
        Instance = this;       
    }

    public override void OnNetworkSpawn()
    {
        state.OnValueChanged += State_OnValueChanged;
        if (IsServer)
        {
            NetworkManager.Singleton.SceneManager.OnLoadEventCompleted += SceneManager_OnLoadEventCompleted;
        }
    }

    private void State_OnValueChanged(GameState previousValue, GameState newValue)
    {
        OnStateChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SceneManager_OnLoadEventCompleted(string sceneName, UnityEngine.SceneManagement.LoadSceneMode loadSceneMode, List<ulong> clientsCompleted, List<ulong> clientsTimedOut)
    {
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            Transform player = Instantiate(playerPrefab);
            player.GetComponent<NetworkObject>().SpawnAsPlayerObject(clientId, true);
        }
    }

    private void Update()
    {

        if (!IsServer)
        {
            return;
        }
        switch (state.Value)
        {
            case GameState.WaitingToStart:
                waitingToStartTimer.Value -= Time.deltaTime;
                if (waitingToStartTimer.Value < 0f)
                {
                    state.Value = GameState.CountdownToStart;
                    
                }
                break;
            case GameState.CountdownToStart:
                countdownToStartTimer.Value -= Time.deltaTime;
                if (countdownToStartTimer.Value < 0f)
                {
                    state.Value = GameState.GamePlaying;
                }
                break;
            case GameState.GamePlaying:
                gamePlayingTimer.Value -= Time.deltaTime;
                if (gamePlayingTimer.Value < 0f)
                {
                    state.Value = GameState.GameOver;
                    
                }
                break;
            case GameState.GameOver:
                break;
        }
    }

    public bool IsGamePlaying()
    {
        return state.Value == GameState.GamePlaying;
    }

    public bool IsCountdownToStartActive()
    {
        return state.Value == GameState.CountdownToStart;
    }

    public float GetCountdownToStartTimer()
    {
        return countdownToStartTimer.Value;
    }

    public bool IsGameOver()
    {
        return state.Value == GameState.GameOver;
    }


}
