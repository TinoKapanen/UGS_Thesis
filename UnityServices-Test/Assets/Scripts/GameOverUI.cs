using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI targetsDestroyedText;
    [SerializeField] private Button returnToLobbyButton;

    private void Awake()
    {
        returnToLobbyButton.onClick.AddListener(() =>
        {
            UnityServicesGameLobby.Instance.ReconnectToLobby();
            Loader.Load(Loader.Scene.PlayerReadyScene);
        });
    }

    private void Start()
    {
        GameManager.Instance.OnStateChanged += GameManager_OnStateChanged;

        Hide();
    }

    private void GameManager_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameManager.Instance.IsGameOver())
        {
            Show();

            targetsDestroyedText.text = ShootingController.Instance.GetDestroyedTargetsAmount().ToString();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
