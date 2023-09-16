using UnityEngine;
using System.Collections;

namespace Pathfinding
{
    public class AICharge : VersionedMonoBehaviour
    {
        public float chargeDistance;

        public float chargePower = 0;

        public Transform target;

        bool canCharge = true;

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
                ai.destination = target.position;

                if (canCharge)
                    Charge();
            }
        }

        void Charge()
        {
            float distance = Vector3.Distance(transform.position, target.position);
            float angle = Vector3.Angle(transform.up, target.position - transform.position);
            if (distance <= chargeDistance && angle <= 5)
            {
                Rigidbody2D rb = GetComponent<Rigidbody2D>();
                rb.AddForce(transform.up * chargePower * rb.mass, ForceMode2D.Impulse);

                General.StartCallbackAfterTime(this, () => canCharge = false, 2.5f, () => canCharge = true);
            }
        }
    }
}
