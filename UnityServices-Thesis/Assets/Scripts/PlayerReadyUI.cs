using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;

public class PlayerReadyUI : MonoBehaviour
{
    [SerializeField] private Button readyButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private TextMeshProUGUI lobbyNameText;
    [SerializeField] private TextMeshProUGUI lobbyCodeText;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            UnityServicesGameLobby.Instance.LeaveLobby();
            NetworkManager.Singleton.Shutdown();
            Loader.Load(Loader.Scene.MainMenu);
        });

        readyButton.onClick.AddListener(() =>
        {
            PlayerSelectReady.Instance.SetPlayerReady();
        });
    }

    private void Start()
    {
        Lobby lobby = UnityServicesGameLobby.Instance.GetLobby();
        
        lobbyNameText.text = "Lobby name: " + lobby.Name;
        lobbyCodeText.text = "Lobby code: " + lobby.LobbyCode;
        
    }
}
