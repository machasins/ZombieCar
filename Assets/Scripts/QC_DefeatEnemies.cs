using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QC_DefeatEnemies : QuestCompletion
{
    private Health[] enemies;
    private bool hasDefeatedAll;

    private void Awake()
    {
        enemies = GetComponentsInChildren<Health>();
        hasDefeatedAll = false;
    }

    private void Update()
    {
        if (!hasDefeatedAll)
        {
            bool allDefeated = true;
            foreach (Health enemy in enemies)
                allDefeated = allDefeated && !enemy.IsAlive();

            if (allDefeated)
            {
                Complete();
                hasDefeatedAll = true;
            }
        }
    }
}
