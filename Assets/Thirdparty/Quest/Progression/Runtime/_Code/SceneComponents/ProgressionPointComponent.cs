using UnityEngine;
using UnityEngine.Events;

namespace Progression {
    /// <summary>
    /// Forwards the completion event of the specified progression point so that it can be used with scene objects.
    /// </summary>
    public class ProgressionPointComponent : MonoBehaviour
    {
        public ProgressionPointAsset progressionPoint;

        public UnityEvent OnComplete = new UnityEvent();

        private UnityAction completeInvoke;

        /// <summary>
        /// Complete the progression point
        /// </summary>
        public void Complete()
        {
            if(progressionPoint)
                progressionPoint.Complete();
        }

        /// <summary>
        /// Save the progression point to a slot
        /// </summary>
        /// <param name="slot"></param>
        public void SaveToSlot(int slot)
        {
            if(progressionPoint)
                progressionPoint.SaveToSlot(slot);
        }

        /// <summary>
        /// Load the progression point from a slot.
        /// </summary>
        /// <param name="slot"></param>
        public void LoadFromSlot(int slot)
        {
            if(progressionPoint)
                progressionPoint.LoadFromSlot(slot);
        }

        private void OnEnable()
        {
            if (progressionPoint == null)
                return;
            completeInvoke = OnComplete.Invoke;
            progressionPoint.OnCompleted.AddListener(completeInvoke);
        }

        private void OnDisable()
        {
            if (progressionPoint == null || completeInvoke == null)
                return;
            progressionPoint.OnCompleted.RemoveListener(completeInvoke);
        }
    }
}