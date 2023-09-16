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
    public class AIPointSetter : VersionedMonoBehaviour
    {
        /// <summary>Current target location</summary>
        public Vector3 point;

        IAstarAI agent;

        protected override void Awake()
        {
            base.Awake();
            agent = GetComponent<IAstarAI>();
        }

        /// <summary>Update is called once per frame</summary>
        void Update()
        {
            agent.destination = point;
        }
    }
}
