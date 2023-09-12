using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Health))]
public class Upgrade : MonoBehaviour
{
    [Header("Upgrades are additive.")]
    [Header("Any Fields that should not be changed, set to NaN.")]
    public bool doCarController;
    public bool doHealth;
    private CarController carController;
    private Health health;

    private static WorldComponentReference wcr;

    private void Awake()
    {
        carController = GetComponent<CarController>();
        health = GetComponent<Health>();

        carController.enabled = false;
        health.enabled = false;

        if (wcr == null)
            wcr = FindObjectOfType<WorldComponentReference>();
    }

    private void OnEnable()
    {
        if (doCarController)
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

        if (doHealth)
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
        if (doCarController)
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

        if (doHealth)
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
}
