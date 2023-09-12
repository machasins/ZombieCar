using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class UpgradeMenuHandler : MonoBehaviour
{
    public GameObject playerUpgrades;
    public GameObject equipmentMenu;

    public GameObject buttonPrefab;
}

[CustomEditor(typeof(UpgradeMenuHandler))]
public class UpgradeMenuHandlerEditor : Editor
{
    public SerializedProperty playerUpgrades;
    public SerializedProperty equipmentMenu;
    public SerializedProperty buttonPrefab;

    private void OnEnable()
    {
        playerUpgrades = serializedObject.FindProperty("playerUpgrades");
        equipmentMenu = serializedObject.FindProperty("equipmentMenu");
        buttonPrefab = serializedObject.FindProperty("buttonPrefab");
    }

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (equipmentMenu.objectReferenceValue
            && playerUpgrades.objectReferenceValue
            && buttonPrefab.objectReferenceValue
            && GUILayout.Button("Remake Buttons"))
        {
            GameObject menu = equipmentMenu.objectReferenceValue as GameObject;
            GameObject player = playerUpgrades.objectReferenceValue as GameObject;
            GameObject button = buttonPrefab.objectReferenceValue as GameObject;

            //foreach (Transform t in menu.transform)
            //    DestroyImmediate(t.gameObject);

            foreach (Transform type in player.transform)
            {
                GameObject t = Instantiate(new GameObject(), menu.transform);
                t.name = type.name;
                foreach (Transform upgrade in type)
                {
                    GameObject u = Instantiate(button, t.transform);
                    u.name = upgrade.name;
                }
            }
        }
    }
}
