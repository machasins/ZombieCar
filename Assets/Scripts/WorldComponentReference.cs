using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class WorldComponentReference : MonoBehaviour
{
    public GameObject player;
    public GameObject cameraPosition;
    public GameObject upgradeMenu;

    public InputActionReference weaponFireInput;
    public InputActionReference interactInput;

    public void ToggleUpgradeUI(bool state)
    {
        player.SetActive(!state);
        player.GetComponent<UpgradeSystem>().ReadUpgrades();
        upgradeMenu.SetActive(state);
        cameraPosition.GetComponent<PlayerInput>().SwitchCurrentActionMap(state ? "UI" : "Player");
    }
}
