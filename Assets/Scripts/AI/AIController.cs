using Pathfinding;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(AIPath))]
public class AIController : MonoBehaviour
{
    public enum State
    { 
        passive,
        hostile,
        breathe
    };

    public enum PassiveAction
    {
        idle,
        patrol,
        wander
    };

    // [[Passive Settings]]
    [Tooltip("What action to take when not in combat")]
    public PassiveAction passive;
        [Tooltip("Percent of max speed to use when passive")]
        [Range(0.0f, 1.0f)] public float passiveSpeed = 1.0f;

        [Tooltip("Patrol points to cycle through")]
        public List<Transform> patrolPoints;

        [Tooltip("Maximum distance to wander from the starting point")]
        public float wanderDistance = 10.0f;
        [Tooltip("Wait time when reaching passive location")]
        public float delay = 3.0f;

    //[[ Hostile Settings ]]
        [Tooltip("How far away can the enemy see the player")]
        public float spotRange = 20.0f;
        [Tooltip("How far away can the enemy notify other enemies that the player is near")]
        public float rallyRange = 15.0f;
        [Tooltip("How far away the player can be before immediately losing aggro")]
        public float loseRange = 30.0f;
        [Tooltip("Percent of max speed to use when hostile")]
        [Range(0.0f, 1.0f)] public float hostileSpeed = 1.0f;
    [Tooltip("Closing distance gap and engaging in combat")]
    public bool doPursuit = true;
    [Tooltip("Rushing for physical attack")]
    public bool doCharge = false;
        [Tooltip("How far away to start the charge")]
        public float chargeDistance = 2.0f;
        [Tooltip("The Power of the charge")]
        public float chargePower = 20.0f;
    [Tooltip("Riding alongside the player, avoiding being in front or behind")]
    public bool doFollow = true;
        [Tooltip("How far to follow")]
        public float followDistance = 2.0f;
    [Tooltip("Moves behind the player")]
    public bool doFlank = true;
        [Tooltip("How far to flank")]
        public float flankDistance = 2.0f;
    [Tooltip("Approach to attack, then run away")]
    public bool doHitNRun = false;
        [Tooltip("Approach to attack, then run away")]
        public float nearDistance = 2.0f;
        [Tooltip("Approach to attack, then run away")]
        public float nearTime = 3.5f;
        [Tooltip("Approach to attack, then run away")]
        public float farDistance = 10.0f;
        [Tooltip("Approach to attack, then run away")]
        public float farTime = 3.5f;

    //[[ Break Settings ]]
        [Tooltip("Distance away from the player that should be maintained")]
        public float breakDistance = 10.0f;
        [Tooltip("Percent of max health that needs to be dealt before running away")]
        public float percentLost;
    [Tooltip("Circle around the player")]
    public bool doCircle = true;
    [Tooltip("Follow from a larger distance and shoot")]
    public bool doDistance = true;
    [Tooltip("Does not engage player, looks from afar")]
    public bool doStalk = true;
    [Tooltip("Focused on moving fancily, not combat")]
    public bool doStyle = false;
    [Tooltip("Attempts to forum up on the leader")]
    public bool doFormation = false;
        [Tooltip("The leader of the formation")]
        public Transform leader;
        [Tooltip("Points the formation should follow")]
        public List<Transform> formation;

    private static WorldComponentReference wcr;

    private AIPath movement;
    private Health health;

    private State state;

    private MonoBehaviour passiveBehavior;
    private AIPointSetter idle;
    private Patrol patrol;
    private Wander wander;

    private List<MonoBehaviour> hostileBehaviors = new();
    private float breatheTime = 0.0f;
    private AIDestinationSetter pursuit;
    private AICharge charge;
    private AIDestinationOffset follow;
    private AIDestinationOffset flank;
    private AIHitNRun hitNRun;

    private List<MonoBehaviour> breatheBehaviors = new();
    private AICircle circle;
    private AIHitNRun distance;
    private AIHitNRun stalk;
    private AIHitNRun style;


    private void Start()
    {
        if (!wcr)
            wcr = FindObjectOfType<WorldComponentReference>();

        movement = GetComponent<AIPath>();
        health = GetComponent<Health>();

        switch (passive)
        {
            case PassiveAction.idle:
                idle = gameObject.AddComponent<AIPointSetter>();
                idle.point = transform.position;

                passiveBehavior = idle;
                break;
            case PassiveAction.patrol:
                patrol = gameObject.AddComponent<Patrol>();
                patrol.targets = patrolPoints.ToArray();
                patrol.delay = delay;

                passiveBehavior = patrol;
                break;
            case PassiveAction.wander:
                wander = gameObject.AddComponent<Wander>();
                wander.distance = wanderDistance;
                wander.delay = delay;

                passiveBehavior = wander;
                break;
        }

        if (doPursuit)
        {
            pursuit = gameObject.AddComponent<AIDestinationSetter>();
            pursuit.target = wcr.player.transform;

            hostileBehaviors.Add(pursuit);
            pursuit.enabled = false;
        }

        if (doCharge)
        {
            charge = gameObject.AddComponent<AICharge>();
            charge.target = wcr.player.transform;
            charge.chargeDistance = chargeDistance;
            charge.chargePower = chargePower;

            hostileBehaviors.Add(charge);
            charge.enabled = false;
        }

        if (doFollow)
        {
            follow = gameObject.AddComponent<AIDestinationOffset>();
            follow.target = wcr.player.transform;
            follow.offsetDistance = followDistance;
            follow.offsetDirection = Vector3.right;
            follow.canMirror = true;

            hostileBehaviors.Add(follow);
            follow.enabled = false;
        }

        if (doFlank)
        {
            flank = gameObject.AddComponent<AIDestinationOffset>();
            flank.target = wcr.player.transform;
            flank.offsetDistance = flankDistance;
            flank.offsetDirection = Vector3.down;
            flank.canMirror = false;

            hostileBehaviors.Add(flank);
            flank.enabled = false;
        }

        if (doHitNRun)
        {
            hitNRun = gameObject.AddComponent<AIHitNRun>();
            hitNRun.target = wcr.player.transform;
            hitNRun.nearDistance = nearDistance;
            hitNRun.nearTime = nearTime;
            hitNRun.farDistance = farDistance;
            hitNRun.farTime = farTime;

            hostileBehaviors.Add(hitNRun);
            hitNRun.enabled = false;
        }

        if (doCircle)
        {
            circle = gameObject.AddComponent<AICircle>();
            circle.target = wcr.player.transform;
            circle.circleRadius = breakDistance;
            circle.SetupPoints();

            breatheBehaviors.Add(circle);
            circle.enabled = false;
        }

        if (doDistance)
        {
            distance = gameObject.AddComponent<AIHitNRun>();
            distance.target = wcr.player.transform;
            distance.nearDistance = breakDistance;
            distance.nearTime = Mathf.Infinity;

            breatheBehaviors.Add(distance);
            distance.enabled = false;
        }

        if (doStalk)
        {
            stalk = gameObject.AddComponent<AIHitNRun>();
            stalk.target = wcr.player.transform;
            stalk.nearDistance = breakDistance;
            stalk.nearTime = Mathf.Infinity;

            breatheBehaviors.Add(stalk);
            stalk.enabled = false;
        }

        if (doStyle)
        {
            style = gameObject.AddComponent<AIHitNRun>();
            style.target = wcr.player.transform;
            style.nearDistance = -breakDistance;
            style.nearTime = Mathf.Infinity;

            breatheBehaviors.Add(style);
            style.enabled = false;
        }

        if (doFormation)
        {

        }

        if (loseRange < spotRange) loseRange = spotRange;
        state = State.passive;
    }

    private void FixedUpdate()
    {
        float distance = Vector3.Distance(transform.position, wcr.player.transform.position);

        switch (state)
        {
            case State.passive:
                SwitchHostile(distance, spotRange, ref breatheTime);
                break;
            case State.hostile:
                SwapHostile(ref breatheTime, hostileBehaviors, breatheBehaviors);
                SwitchPassive(distance, loseRange, hostileBehaviors);
                break;
            case State.breathe:
                SwapHostile(ref breatheTime, breatheBehaviors, hostileBehaviors);
                SwitchPassive(distance, loseRange, breatheBehaviors);
                break;
        }
    }

    private void SwitchHostile(float distance, float range, ref float time)
    {
        if (distance <= range)
        {
            passiveBehavior.enabled = false;
            if (hostileBehaviors.Count <= 0)
            {
                state = State.breathe;
                breatheBehaviors[Random.Range(0, breatheBehaviors.Count)].enabled = true;
            }
            else
            {
                state = State.hostile;
                hostileBehaviors[Random.Range(0, hostileBehaviors.Count)].enabled = true;
            }

            time = Time.time + 20.0f;

            Debug.Log("Current State is " + state.ToString());
        }
    }

    private void SwitchPassive(float distance, float range, List<MonoBehaviour> currentStateBehaviors)
    {
        if (distance >= range)
        {
            foreach (var m in currentStateBehaviors)
                m.enabled = false;
            passiveBehavior.enabled = true;
            state = State.passive;
        }
    }

    private void SwapHostile(ref float time, List<MonoBehaviour> currentStateBehaviors, List<MonoBehaviour> swapStateBehaviors)
    {
        if (Time.time >= time)
        {
            state = (state == State.hostile) ? State.breathe : State.hostile;

            foreach (var m in currentStateBehaviors)
                m.enabled = false;

            if (swapStateBehaviors.Count <= 0)
            {
                SwapHostile(ref time, swapStateBehaviors, currentStateBehaviors);
                return;
            }

            swapStateBehaviors[Random.Range(0, swapStateBehaviors.Count)].enabled = true;

            time = Time.time + 20.0f;

            Debug.Log("Current State is " + state.ToString());
        }
    }
}

[CustomEditor(typeof(AIController))]
public class AIControllerEditor : Editor
{
    SerializedProperty passiveType;

    SerializedProperty passiveSpeed;
    SerializedProperty passivePatrolPoints;
    SerializedProperty passiveDelay;

    SerializedProperty passiveWanderDistance;

    SerializedProperty spotRange;
    SerializedProperty rallyRange;
    SerializedProperty loseRange;
    SerializedProperty hostileSpeed;

    SerializedProperty doPursuit;

    SerializedProperty doCharge;
    SerializedProperty chargeDistance;
    SerializedProperty chargePower;

    SerializedProperty doFollow;
    SerializedProperty followDistance;

    SerializedProperty doFlank;
    SerializedProperty flankDistance;

    SerializedProperty doCircle;

    SerializedProperty doHitNRun;
    SerializedProperty nearDistance;
    SerializedProperty nearTime;
    SerializedProperty farDistance;
    SerializedProperty farTime;

    SerializedProperty breakDistance;
    SerializedProperty percentLost;

    SerializedProperty doDistance;
    SerializedProperty doStalk;
    SerializedProperty doStyle;

    SerializedProperty doFormation;
    SerializedProperty leader;
    SerializedProperty formation;
    bool isLeader = false;

    private void Awake()
    {
        passiveType = serializedObject.FindProperty("passive");
        passiveSpeed = serializedObject.FindProperty("passiveSpeed");

        passivePatrolPoints = serializedObject.FindProperty("patrolPoints");
        passiveDelay = serializedObject.FindProperty("delay");

        passiveWanderDistance = serializedObject.FindProperty("wanderDistance");

        spotRange = serializedObject.FindProperty("spotRange");
        rallyRange = serializedObject.FindProperty("rallyRange");
        loseRange = serializedObject.FindProperty("loseRange");
        hostileSpeed = serializedObject.FindProperty("hostileSpeed");

        doPursuit = serializedObject.FindProperty("doPursuit");

        doCharge = serializedObject.FindProperty("doCharge");
        chargeDistance = serializedObject.FindProperty("chargeDistance");
        chargePower = serializedObject.FindProperty("chargePower");

        doFollow = serializedObject.FindProperty("doFollow");
        followDistance = serializedObject.FindProperty("followDistance");

        doFlank = serializedObject.FindProperty("doFlank");
        flankDistance = serializedObject.FindProperty("flankDistance");


        doHitNRun = serializedObject.FindProperty("doHitNRun");
        nearDistance = serializedObject.FindProperty("nearDistance");
        nearTime = serializedObject.FindProperty("nearTime");
        farDistance = serializedObject.FindProperty("farDistance");
        farTime = serializedObject.FindProperty("farTime");

        breakDistance = serializedObject.FindProperty("breakDistance");
        percentLost = serializedObject.FindProperty("percentLost");

        doCircle = serializedObject.FindProperty("doCircle");
        doDistance = serializedObject.FindProperty("doDistance");
        doStalk = serializedObject.FindProperty("doStalk");
        doStyle = serializedObject.FindProperty("doStyle");

        doFormation = serializedObject.FindProperty("doFormation");
        leader = serializedObject.FindProperty("leader");
        formation = serializedObject.FindProperty("formation");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Passive Settings", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(passiveType);

        switch (passiveType.enumValueFlag)
        {
            case (int)AIController.PassiveAction.idle:
                break;
            case (int)AIController.PassiveAction.patrol:
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(passiveSpeed);
                EditorGUILayout.PropertyField(passivePatrolPoints);
                EditorGUILayout.PropertyField(passiveDelay);
                EditorGUI.indentLevel--;
                break;
            case (int)AIController.PassiveAction.wander:
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(passiveSpeed);
                EditorGUILayout.PropertyField(passiveWanderDistance);
                EditorGUILayout.PropertyField(passiveDelay);
                EditorGUI.indentLevel--;
                break;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("Hostile Settings", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(spotRange);
        EditorGUILayout.PropertyField(rallyRange);
        EditorGUILayout.PropertyField(loseRange);
        EditorGUILayout.PropertyField(hostileSpeed);
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(doPursuit);

        EditorGUILayout.PropertyField(doCharge);
        if (doCharge.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(chargeDistance);
            EditorGUILayout.PropertyField(chargePower);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(doFollow);
        if (doFollow.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(followDistance);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(doFlank);
        if (doFlank.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(flankDistance);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.PropertyField(doHitNRun);
        if (doHitNRun.boolValue)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.PropertyField(nearDistance);
            EditorGUILayout.PropertyField(nearTime);
            EditorGUILayout.PropertyField(farDistance);
            EditorGUILayout.PropertyField(farTime);
            EditorGUI.indentLevel--;
        }

        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);

        EditorGUILayout.LabelField("Break Settings", EditorStyles.boldLabel);

        EditorGUI.indentLevel++;
        EditorGUILayout.PropertyField(breakDistance);
        EditorGUILayout.PropertyField(percentLost);
        EditorGUI.indentLevel--;

        EditorGUILayout.PropertyField(doCircle);
        EditorGUILayout.PropertyField(doDistance);
        EditorGUILayout.PropertyField(doStalk);
        EditorGUILayout.PropertyField(doStyle);

        EditorGUILayout.PropertyField(doFormation);
        if (doFormation.boolValue)
        {
            EditorGUI.indentLevel++;
            isLeader = EditorGUILayout.Toggle("Is Leader", isLeader);
            if (isLeader)
                EditorGUILayout.PropertyField(formation);
            else
                EditorGUILayout.PropertyField(leader);
            EditorGUI.indentLevel--;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
