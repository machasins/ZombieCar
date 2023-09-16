using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    /// <summary>
    /// Simple patrol behavior.
    /// This will set the destination on the agent so that it moves through the sequence of objects in the <see cref="targets"/> array.
    /// Upon reaching a target it will wait for <see cref="delay"/> seconds.
    ///
    /// See: <see cref="Pathfinding.AIDestinationSetter"/>
    /// See: <see cref="Pathfinding.AIPath"/>
    /// See: <see cref="Pathfinding.RichAI"/>
    /// See: <see cref="Pathfinding.AILerp"/>
    /// </summary>
    [UniqueComponent(tag = "ai.destination")]
    [HelpURL("http://arongranberg.com/astar/docs/class_pathfinding_1_1_patrol.php")]
    public class Wander : VersionedMonoBehaviour
    {
        /// <summary>Maximum distance away from the starting point</summary>
        public float distance;

        /// <summary>Time in seconds to wait at each target</summary>
        public float delay = 0;

        /// <summary>Current target location</summary>
        Vector2 point;
        Vector3 startingPosition;

        IAstarAI agent;
        float switchTime = float.PositiveInfinity;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<IAstarAI>();

            startingPosition = transform.position;
            point = startingPosition + (Vector3)(Random.insideUnitCircle * distance);
        }

        /// <summary>Update is called once per frame</summary>
        void Update()
        {
            bool search = false;

            // Note: using reachedEndOfPath and pathPending instead of reachedDestination here because
            // if the destination cannot be reached by the agent, we don't want it to get stuck, we just want it to get as close as possible and then move on.
            if (agent.reachedEndOfPath && !agent.pathPending && float.IsPositiveInfinity(switchTime))
                switchTime = Time.time + delay;

            if (Time.time >= switchTime)
            {
                point = startingPosition + (Vector3)(Random.insideUnitCircle * distance);

                search = true;
                switchTime = float.PositiveInfinity;
            }

            agent.destination = point;

            if (search) agent.SearchPath();
        }
    }
}
