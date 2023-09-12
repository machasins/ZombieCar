using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UpgradeStationTrigger : MonoBehaviour
{
    private static WorldComponentReference wcr;

    private void Awake()
    {
        if (wcr == null)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject == wcr.player && wcr.interactInput.action.WasPressedThisFrame())
            wcr.ToggleUpgradeUI(true);
    }
}
