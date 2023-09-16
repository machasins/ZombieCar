using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeStationTrigger : MonoBehaviour
{
    private static WorldComponentReference wcr;

    private bool playerCollided = false;

    private void Awake()
    {
        if (wcr == null)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    private void Update()
    {
        if (playerCollided && wcr.interactInput.action.WasPerformedThisFrame())
            wcr.ToggleUpgradeUI(true);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CollisionCheck(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CollisionCheck(collision, false);
    }

    private void CollisionCheck(Collider2D collision, bool isEntering)
    {
        if (collision.gameObject == wcr.player)
            playerCollided = isEntering;
    }
}
