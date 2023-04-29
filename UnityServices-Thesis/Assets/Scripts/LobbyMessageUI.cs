using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMessageUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI messageText;
    [SerializeField] private Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Hide);

    }

    private void Start()
    {
        GameMultiplayer.Instance.OnFailedToJoinGame += GameMultiplayer_OnFailedToJoinGame;
        UnityServicesGameLobby.Instance.OnCreateLobbyStarted += UnityServicesGameLobby_OnCreateLobbyStarted;
        UnityServicesGameLobby.Instance.OnCreateLobbyFailed += UnityServicesGameLobby_OnCreateLobbyFailed;
        UnityServicesGameLobby.Instance.OnJoinStarted += UnityServicesGameLobby_OnJoinStarted;
        UnityServicesGameLobby.Instance.OnJoinFailed += UnityServicesGameLobby_OnJoinFailed;
        UnityServicesGameLobby.Instance.OnQuickJoinFailed += UnityServicesGameLobby_OnQuickJoinFailed;

        Hide();
    }

    private void UnityServicesGameLobby_OnQuickJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Could not find a lobby to join!");
    }

    private void UnityServicesGameLobby_OnJoinFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to join lobby!");
    }

    private void UnityServicesGameLobby_OnJoinStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Joining lobby...");
    }

    private void UnityServicesGameLobby_OnCreateLobbyFailed(object sender, System.EventArgs e)
    {
        ShowMessage("Failed to create lobby!");
    }

    private void UnityServicesGameLobby_OnCreateLobbyStarted(object sender, System.EventArgs e)
    {
        ShowMessage("Creating lobby...");
    }

    private void GameMultiplayer_OnFailedToJoinGame(object sender, System.EventArgs e)
    {
        if (NetworkManager.Singleton.DisconnectReason == "")
        {
            ShowMessage("Failed to connect");
        }
        else
        {
            ShowMessage(NetworkManager.Singleton.DisconnectReason);
        }
    }

    private void ShowMessage(string message)
    {
        Show();
        messageText.text = message;
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameMultiplayer.Instance.OnFailedToJoinGame -= GameMultiplayer_OnFailedToJoinGame;
        UnityServicesGameLobby.Instance.OnCreateLobbyStarted -= UnityServicesGameLobby_OnCreateLobbyStarted;
        UnityServicesGameLobby.Instance.OnCreateLobbyFailed -= UnityServicesGameLobby_OnCreateLobbyFailed;
        UnityServicesGameLobby.Instance.OnJoinStarted -= UnityServicesGameLobby_OnJoinStarted;
        UnityServicesGameLobby.Instance.OnJoinFailed -= UnityServicesGameLobby_OnJoinFailed;
        UnityServicesGameLobby.Instance.OnQuickJoinFailed -= UnityServicesGameLobby_OnQuickJoinFailed;
    }
}
