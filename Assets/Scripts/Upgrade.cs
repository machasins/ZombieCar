using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class Upgrade : MonoBehaviour
{
    [Header("Upgrades are additive.")]
    [Header("Any Fields that should not be changed, set to NaN.")]
    public bool doCarController;
    public bool doHealth;
    public string title = "";
    [TextArea()] public string description = "";
    private CarController carController;
    private Health health;

    private static WorldComponentReference wcr;

    private void Awake()
    {
        if (doCarController)
        {
            carController = GetComponent<CarController>();
            carController.enabled = false;
        }

        if (doHealth)
        {
            health = GetComponent<Health>();
            health.enabled = false;
        }

        if (wcr == null)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    private void OnEnable()
    {
        if (doCarController && wcr.player)
        {
            CarController playerCar = wcr.player.GetComponent<CarController>();
            foreach (FieldInfo prop in playerCar.GetType().GetFields())
            {
                if (prop.FieldType == typeof(float))
                {
                    float playerVal = (float)prop.GetValue(playerCar);
                    float upgradeVal = (float)prop.GetValue(carController);
                    if (!float.IsNaN(upgradeVal))
                        prop.SetValue(playerCar, playerVal + upgradeVal);
                }
            }
        }

        if (doHealth && wcr.player)
        {
            Health playerCar = wcr.player.GetComponent<Health>();
            foreach (FieldInfo prop in playerCar.GetType().GetFields())
            {
                if (prop.FieldType == typeof(float))
                {
                    float playerVal = (float)prop.GetValue(playerCar);
                    float upgradeVal = (float)prop.GetValue(health);
                    if (!float.IsNaN(upgradeVal))
                        prop.SetValue(playerCar, playerVal + upgradeVal);
                }
            }
        }
    }

    private void OnDisable()
    {
        if (doCarController && wcr.player)
        {
            CarController playerCar = wcr.player.GetComponent<CarController>();
            foreach (FieldInfo prop in carController.GetType().GetFields())
            {
                if (prop.FieldType == typeof(float))
                {
                    float playerVal = (float)prop.GetValue(playerCar);
                    float upgradeVal = (float)prop.GetValue(carController);
                    if (!float.IsNaN(upgradeVal))
                        prop.SetValue(playerCar, playerVal - upgradeVal);
                }
            }
        }

        if (doHealth && wcr.player)
        {
            Health playerCar = wcr.player.GetComponent<Health>();
            foreach (FieldInfo prop in health.GetType().GetFields())
            {
                if (prop.FieldType == typeof(float))
                {
                    float playerVal = (float)prop.GetValue(playerCar);
                    float upgradeVal = (float)prop.GetValue(health);
                    if (!float.IsNaN(upgradeVal))
                        prop.SetValue(playerCar, playerVal - upgradeVal);
                }
            }
        }
    }

    public string GetName()
    {
        return (title == "") ? gameObject.name : title;
    }

    public string GetDesc()
    {
        return (description == "") ? "##DESCRIPTION NOT FOUND##" : description;
    }
}
