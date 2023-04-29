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
    [SerializeField] private TMP_InputField joinCodeInputField;
    [SerializeField] private TMP_InputField playerNameInputField;
    [SerializeField] private LobbyCreateUI lobbyCreateUI;
    [SerializeField] private Transform lobbyContainer;
    [SerializeField] private Transform lobbyTemplate;

    private void Awake()
    {
        mainMenuButton.onClick.AddListener(() =>
        {
            UnityServicesGameLobby.Instance.LeaveLobby();
            Loader.Load(Loader.Scene.MainMenu);
        });
        
        createLobbyButton.onClick.AddListener(() =>
        {
            lobbyCreateUI.Show();
            Hide();

        });
        
        quickJoinButton.onClick.AddListener(() =>
        {
            UnityServicesGameLobby.Instance.QuickJoin();
        });

        joinCodeButton.onClick.AddListener(() =>
        {
            UnityServicesGameLobby.Instance.JoinWithCode(joinCodeInputField.text);
        });

        lobbyTemplate.gameObject.SetActive(false);

    }

    private void Start()
    {
        playerNameInputField.text = GameMultiplayer.Instance.GetPlayerName();
        playerNameInputField.onValueChanged.AddListener((string newText) =>
        {
            GameMultiplayer.Instance.SetPlayerName(newText);
        });

        UnityServicesGameLobby.Instance.OnLobbyListChanged += UnityServicesGameLobby_OnLobbyListChanged;
        UpdateLobbyList(new List<Lobby>());
    }

    private void UnityServicesGameLobby_OnLobbyListChanged(object sender, UnityServicesGameLobby.OnLobbyListChangedEventArgs e)
    {
        UpdateLobbyList(e._lobbyList);
    }

    private void UpdateLobbyList(List<Lobby> lobbyList)
    {
        foreach (Transform child in lobbyContainer)
        {
            if (child == lobbyTemplate) continue;
            Destroy(child.gameObject);
        }

        foreach (Lobby lobby in lobbyList)
        {
            Transform lobbyTransform = Instantiate(lobbyTemplate, lobbyContainer);
            lobbyTransform.gameObject.SetActive(true);
            lobbyTransform.GetComponent<LobbyListSingleUI>().SetLobby(lobby);
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        UnityServicesGameLobby.Instance.OnLobbyListChanged -= UnityServicesGameLobby_OnLobbyListChanged;
    }
}
