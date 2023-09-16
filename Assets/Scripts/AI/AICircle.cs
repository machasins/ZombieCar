using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AICircle : VersionedMonoBehaviour
    {
        /// <summary>The object that the AI should move to</summary>
        public Transform target;

        public float circleRadius;

        private int numPoints = 20;
        private int currentPoint = 0;
        private List<Vector3> circlePoints;

        IAstarAI ai;

        void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
            // Update the destination right before searching for a path as well.
            // This is enough in theory, but this script will also update the destination every
            // frame as the destination is used for debugging and may be used for other things by other
            // scripts as well. So it makes sense that it is up to date every frame.
            if (ai != null) ai.onSearchPath += Update;
        }

        void OnDisable()
        {
            if (ai != null) ai.onSearchPath -= Update;
        }

        /// <summary>Updates the AI's destination every frame</summary>
        void Update()
        {
            if (target != null && ai != null)
            {
                float distanceToTarget = Vector3.Distance(transform.position, target.position + circlePoints[currentPoint]);

                if (distanceToTarget < circleRadius * 0.75)
                    currentPoint = (currentPoint + 1) % numPoints;

                ai.destination = target.position + circlePoints[currentPoint];
            }
        }

        public void SetupPoints()
        {
            circlePoints = new();
            for (int i = 0; i < numPoints; ++i)
            {
                float angle = i * Mathf.PI * 2.0f / numPoints;
                Vector2 point = new(Mathf.Cos(angle) * circleRadius, Mathf.Sin(angle) * circleRadius);

                circlePoints.Add(point);
            }
        }
    }
}
