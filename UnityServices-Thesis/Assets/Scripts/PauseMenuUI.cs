using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PauseMenuUI : MonoBehaviour
{
    private MenuControls _controls;
    private InputAction _menu;

    [SerializeField] private GameObject pauseUI;
    [SerializeField] private bool isPaused;

    [SerializeField] private Button mainMenuButton;

    private void Awake()
    {
        _controls = new MenuControls();

        mainMenuButton.onClick.AddListener(() =>
        {
            Loader.Load(Loader.Scene.MainMenu);
        });
    }

    private void OnEnable()
    {
        _menu = _controls.Menu.Pause;
        _menu.Enable();

        _menu.performed += Pause;
    }

    private void OnDestroy()
    {
        _menu.performed -= Pause;

        _controls.Dispose();
    }

    private void OnDisable()
    {
        _menu.Disable();
    }

    public void Pause(InputAction.CallbackContext context)
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Cursor.lockState = CursorLockMode.None;
            ActivateMenu();
        }
        else
        {
            DeactivateMenu();
            Cursor.lockState = CursorLockMode.Locked;
        }     
    }

    void ActivateMenu()
    {
        pauseUI.SetActive(true);

        mainMenuButton.Select();
    }

    public void DeactivateMenu()
    {
        pauseUI.SetActive(false);
        isPaused = false;
    }
}
