using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Events;
using Unity.VisualScripting;
using System;

public class UpgradeMenuHandler : MonoBehaviour
{
    public GameObject playerUpgrades;
    public GameObject equipmentMenu;
    [Space()]
    public TMP_Text menuText;
    public TMP_Text equipedText;
    public TMP_Text descText;
    [Space()]
    public GameObject buttonPrefab;
    [Space()]
    public int maxWidth;
    public Vector2 offsets;

    private void Start()
    {
        menuText.text = "Equipment";
        equipedText.text = "";
        descText.text = "";

        GameObject upgradeContainer = Instantiate(new GameObject(), equipmentMenu.transform);
        upgradeContainer.name = "Equipment";

        int upgradeType = 0;
        Vector2 offset = Vector2.zero;
        foreach (Transform type in playerUpgrades.transform)
        {
            CreateMainMenuButton(upgradeContainer.transform, type, ref offset, upgradeType);
            ++upgradeType;
        }

        CreateExitButton(upgradeContainer.transform, offset);
    }

    private void CreateMainMenuButton(Transform parent, Transform upgradeType, ref Vector2 offset, int upgradeIndex)
    {
        // Main Menu Button
        GameObject mainMenuButton = Instantiate(buttonPrefab, parent);
        mainMenuButton.name = upgradeType.name;
        mainMenuButton.GetComponentInChildren<TMP_Text>().text = upgradeType.name;
        mainMenuButton.GetComponent<RectTransform>().anchoredPosition = offset;

        offset = ((offset.x + offsets.x) / (maxWidth * offsets.x) < 1.0f)
            ? new Vector2(offset.x + offsets.x, offset.y)
            : new Vector2(0, offset.y + offsets.y);

        // Sub Menu Buttons
        GameObject subMenuContainer = Instantiate(new GameObject(), equipmentMenu.transform);
        subMenuContainer.name = upgradeType.name;

        int index = 0;
        Vector2 currOffset = Vector2.zero;
        foreach (Transform upgrade in upgradeType)
        {
            CreateSubMenuButton(upgrade, subMenuContainer.transform, ref currOffset, new(upgradeIndex, index));
            ++index;
        }

        CreateSubBackButton(subMenuContainer.transform, parent.gameObject, currOffset);

        // Set up Main Menu Button
        Button b = mainMenuButton.GetComponent<Button>();
        b.onClick.AddListener(delegate { parent.gameObject.SetActive(false); });
        b.onClick.AddListener(delegate { subMenuContainer.SetActive(true); });
        b.onClick.AddListener(delegate { menuText.text = subMenuContainer.name; });
        b.onClick.AddListener(delegate { 
            Upgrade u = upgradeType.GetChild(playerUpgrades.GetComponentInParent<UpgradeSystem>().GetUpgrade(upgradeIndex)).GetComponent<Upgrade>(); 
            equipedText.text = (u) ? u.GetName() : "##NAME NOT FOUND##"; });
        b.onClick.AddListener(delegate { 
            Upgrade u = upgradeType.GetChild(playerUpgrades.GetComponentInParent<UpgradeSystem>().GetUpgrade(upgradeIndex)).GetComponent<Upgrade>();
            descText.text = (u) ? u.GetDesc() : "##DESCRIPTION NOT FOUND##"; });

        subMenuContainer.SetActive(false);
    }

    private void CreateSubMenuButton(Transform upgrade, Transform parent, ref Vector2 currOffset, Vector2Int upgradeIndex)
    {
        // Sub Menu Button
        GameObject u = Instantiate(buttonPrefab, parent);
        u.name = upgrade.name;
        u.GetComponentInChildren<TMP_Text>().text = upgrade.name;
        u.GetComponent<RectTransform>().anchoredPosition = currOffset;

        currOffset = ((currOffset.x + offsets.x) / (maxWidth * offsets.x) < 1.0f)
            ? new Vector2(currOffset.x + offsets.x, currOffset.y)
            : new Vector2(0, currOffset.y + offsets.y);

        // Variables for Button
        Upgrade upgradeInfo = upgrade.GetComponent<Upgrade>();
        string title = (upgradeInfo && upgradeInfo.title != "") ? upgradeInfo.title : u.name;
        string desc = (upgradeInfo && upgradeInfo.description != "") ? upgradeInfo.description : "##Description not found##";

        // Set up Sub Menu Button
        Button sub = u.GetComponent<Button>();
        sub.onClick.AddListener(delegate { playerUpgrades.GetComponentInParent<UpgradeSystem>().ChangeUpgrade(upgradeIndex.x, upgradeIndex.y); });
        sub.onClick.AddListener(delegate { equipedText.text = title; });
        sub.onClick.AddListener(delegate { descText.text = desc; });
    }

    private void CreateSubBackButton(Transform parent, GameObject equip, Vector2 currOffset)
    {
        // Back button Button
        GameObject bb = Instantiate(buttonPrefab, parent);
        bb.name = "Back";
        bb.GetComponentInChildren<TMP_Text>().text = "Back";
        bb.GetComponent<RectTransform>().anchoredPosition = currOffset;
        bb.GetComponent<Image>().color = Color.white;

        // Set up Back Button
        Button b = bb.GetComponent<Button>();
        b.onClick.AddListener(delegate { equip.SetActive(true); });
        b.onClick.AddListener(delegate { parent.gameObject.SetActive(false); });
        b.onClick.AddListener(delegate { menuText.text = "Equipment"; });
        b.onClick.AddListener(delegate { equipedText.text = ""; });
        b.onClick.AddListener(delegate { descText.text = ""; });
    }

    private void CreateExitButton(Transform parent, Vector2 offset)
    {
        // Exit button Button
        GameObject exit = Instantiate(buttonPrefab, parent);
        exit.name = "Exit";
        exit.GetComponentInChildren<TMP_Text>().text = "Exit";
        exit.GetComponent<RectTransform>().anchoredPosition = offset;
        exit.GetComponent<Image>().color = Color.white;

        // Set up Exit Button
        exit.GetComponent<Button>().onClick.AddListener(delegate { FindObjectOfType<WorldComponentReference>().ToggleUpgradeUI(false); });
    }
}
