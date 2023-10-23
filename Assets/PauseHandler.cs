using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseHandler : MonoBehaviour
{
    public InputActionReference pauseButtonPlayer;
    public InputActionReference pauseButtonUI;

    private static WorldComponentReference wcr;

    private CanvasGroup group;

    private bool isPaused = false;

    private void Start()
    {
        if (wcr == null)
            wcr = FindObjectOfType<WorldComponentReference>();

        group = GetComponent<CanvasGroup>();

        pauseButtonPlayer.action.performed += OnPause;
        pauseButtonUI.action.performed += OnPause;
    }

    private void OnPause(InputAction.CallbackContext context)
    {
        if (context.action.WasPressedThisFrame())
            TogglePause(!isPaused);
    }

    public void TogglePause(bool active)
    {
        wcr.cameraPosition.GetComponent<PlayerInput>().SwitchCurrentActionMap(active ? "UI" : "Player");

        pauseButtonPlayer.action.performed -= OnPause;
        pauseButtonUI.action.performed -= OnPause;
        pauseButtonPlayer.action.performed += OnPause;
        pauseButtonUI.action.performed += OnPause;

        group.DOFade((active) ? 1.0f : 0.0f, 0.25f).SetUpdate(true);
        group.interactable = active;
        group.blocksRaycasts = active;

        Time.timeScale = (active) ? 0.0f : 1.0f;

        isPaused = active;
    }
}
