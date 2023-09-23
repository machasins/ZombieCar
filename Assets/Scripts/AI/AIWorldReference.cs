using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIWorldReference : MonoBehaviour
{
    [Header("Time to switch between hostile and breathe states")]
    public float minSwitchTime = 20.0f;
    [Tooltip("Maximum amount of additional time from the minimum")]
    public float maxSwitchVariance = 10.0f;

    [Header("Time to switch between behaviors within a state")]
    public float minBehaviorSwitchTime = 5.0f;
    [Tooltip("Maximum amount of additional time from the minimum")]
    public float maxBehaviorSwitchVariance = 5.0f;

    [Header("How likely certain AI behaviors are to be chosen.")]
    public int pursuitLikelyhood = 2;
    public int chargeLikelyhood = 1;
    public int followLikelyhood = 2;
    public int flankLikelyhood = 2;
    public int hitNRunLikelyhood = 1;

    [Space()]
    public int circleLikelyhood = 2;
    public int distanceLikelyhood = 2;
    public int stalkLikelyhood = 1;
    public int styleLikelyhood = 3;

    [Header("Modifiers for Intelligence and Skill.")]
    [Header("Percent of the base chance to be added when Intelligence is high. \n Negative values indicate that it is more likely the less Intelligent the AI is.")]
    public float pursuitIntelMult =  0.0f;
    public float chargeIntelMult =  0.3f;
    public float followIntelMult = -0.1f;
    public float flankIntelMult =  0.0f;
    public float hitNRunIntelMult =  0.3f;

    [Space()]
    public float circleIntelMult = 0.0f;
    public float distanceIntelMult = -0.1f;
    public float stalkIntelMult =  0.2f;
    public float styleIntelMult = -0.3f;

    [Header("Percent of the base chance to be added when Skill is high. \n Negative values indicate that it is more likely the less Skilled the AI is.")]
    public float pursuitSkillMult =  0.0f;
    public float chargeSkillMult =  0.2f;
    public float followSkillMult = -0.3f;
    public float flankSkillMult =  0.1f;
    public float hitNRunSkillMult =  0.0f;

    [Space()]
    public float circleSkillMult =  0.1f;
    public float distanceSkillMult = -0.3f;
    public float stalkSkillMult = -0.3f;
    public float styleSkillMult =  0.3f;
}
