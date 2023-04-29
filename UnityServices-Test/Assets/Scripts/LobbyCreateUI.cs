using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyCreateUI : MonoBehaviour
{
    [SerializeField] private Button closeButton;
    [SerializeField] private Button createButton;
    [SerializeField] private Toggle publicPrivateToggle;
    [SerializeField] private TMP_InputField lobbyNameInputField;
    [SerializeField] private LobbyUI lobbyUI;

    private bool _isPrivate;

    private void Awake()
    {
        createButton.onClick.AddListener(() =>
        {
            if (!_isPrivate)
            {
                UnityServicesGameLobby.Instance.CreateLobby(lobbyNameInputField.text, false);
            }
            else
            {
                UnityServicesGameLobby.Instance.CreateLobby(lobbyNameInputField.text, true);
            }
        });

        publicPrivateToggle.onValueChanged.AddListener((on) =>
        {
            if (!on)
            {
                _isPrivate = false;
            }
            else
            {
                _isPrivate = true;
            }
        });

        closeButton.onClick.AddListener(() =>
        {
            lobbyUI.Show();
            Hide();
        });
    }

    private void Start()
    {
        Hide();
    }

    public void Show()
    {
        gameObject.SetActive(true);

        publicPrivateToggle.Select();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
