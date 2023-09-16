using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class AIHitNRun : VersionedMonoBehaviour
    {
        /// <summary>The object that the AI should move to</summary>
        public Transform target;

        public float nearDistance;
        public float nearTime;
        public float farDistance;
        public float farTime;

        private bool isNear = true;
        private float switchTime;

        IAstarAI ai;

        void OnEnable()
        {
            ai = GetComponent<IAstarAI>();
            // Update the destination right before searching for a path as well.
            // This is enough in theory, but this script will also update the destination every
            // frame as the destination is used for debugging and may be used for other things by other
            // scripts as well. So it makes sense that it is up to date every frame.
            if (ai != null) ai.onSearchPath += Update;

            isNear = true;
            switchTime = Time.time + nearTime;
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
                if (isNear)
                {
                    ai.destination = target.position + (transform.position - target.position).normalized * nearDistance;

                    if (Time.time > switchTime)
                    {
                        isNear = false;
                        switchTime = Time.time + farTime;
                    }
                }
                else
                {
                    ai.destination = target.position + (transform.position - target.position).normalized * farDistance;

                    if (Time.time > switchTime)
                    {
                        isNear = true;
                        switchTime = Time.time + nearTime;
                    }
                }
            }
        }
    }
}