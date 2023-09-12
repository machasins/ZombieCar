using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeSystem : MonoBehaviour
{
    public List<GameObject> engines;
    public List<GameObject> tires;
    public List<GameObject> guns;
    public List<GameObject> grills;
    public List<GameObject> doors;

    private List<List<GameObject>> upgrades;
    private List<int> activeUpgrades;

    private readonly List<string> upgradeTypes = new()
    {
        "Engine",
        "Tire",
        "Gun",
        "Grill",
        "Door"
    };

    private void Awake()
    {
        upgrades = new()
        {
            engines,
            tires,
            guns,
            grills,
            doors
        };

        ReadUpgrades();
    }

    public void ReadUpgrades()
    {
        activeUpgrades = new();

        foreach (string type in upgradeTypes)
            activeUpgrades.Add(PlayerPrefs.GetInt("upgrade" + type + "Type", 0));

        for (int i = 0; i < activeUpgrades.Count; ++i)
            for (int j = 0; j < upgrades[i].Count; ++j)
                upgrades[i][j].SetActive(j == activeUpgrades[i]);
    }

    private void ChangeUpgrade(int type, int index)
    {
        PlayerPrefs.SetInt("upgrade" + upgradeTypes[type] + "Type", index);
        activeUpgrades[type] = index;

        for (int j = 0; j < upgrades[type].Count; ++j)
            upgrades[type][j].SetActive(j == index);
    }

    public void ChangeEngine(int index)
    {
        ChangeUpgrade(0, index);
    }

    public void ChangeTire(int index)
    {
        ChangeUpgrade(1, index);
    }

    public void ChangeGun(int index)
    {
        ChangeUpgrade(2, index);
    }

    public void ChangeGrill(int index)
    {
        ChangeUpgrade(3, index);
    }

    public void ChangeDoor(int index)
    {
        ChangeUpgrade(4, index);
    }
}
