using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AITraits : MonoBehaviour
{
    [Tooltip("How likely the enemy is to engage in combat.")]
    [Range(0.0f, 1.0f)] public float aggressive = 0.5f;
    [Tooltip("How likely the enemy is to be strategic or work with others.")]
    [Range(0.0f, 1.0f)] public float intelligence = 0.5f;
    [Tooltip("How likely the enemy is to wait to attack.")]
    [Range(0.0f, 1.0f)] public float patience = 0.5f;
    [Tooltip("How well the enemy aims and maneuvers.")]
    [Range(0.0f, 1.0f)] public float skill = 0.5f;
    [Tooltip("How likely the enemy is to repeat actions already taken.")]
    [Range(0.0f, 1.0f)] public float predictive = 0.5f;
    [Tooltip("How much speed and acceleration the enemy uses.")]
    [Range(0.0f, 1.0f)] public float reckless = 0.5f;
    [Tooltip("How likely the enemy is to continue attacking.")]
    [Range(0.0f, 1.0f)] public float persistent = 0.5f;

    /*
     * - Agressive - 
     * How commonly the enemy changes up it's attacks, how long it stays in the attack mode.
     * 
     * - Intelligence -
     * Determines requirements for certain maneuvers, chance of leaving at low health, prediction aiming
     * 
     * - Patience -
     * How long the enemy stays in the beathe mode
     * 
     * - Skill -
     * Speed of aiming mechanism, determines requirements for certain maneuvers
     * 
     * - Predictive -
     * How commonly the same move is used twice in a row
     * 
     * - Reckless -
     * Speed of car, acceleration of car, drifting threshold
     * 
     * - Persistent -
     * Chance of the enemy to give up attacking after a long time or distance
     */
}
