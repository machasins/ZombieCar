using UnityEngine;
using UnityEngine.Events;

namespace Progression {
    /// <summary>
    /// Useful for triggering in game events when a quest dependency is satisfied.
    /// </summary>
    public class QuestDependencyComponent : MonoBehaviour
    {
        [SerializeField]
        private Dependency questDependency = null;


        [Header("Satisfy() has been called on the Quest Dependency.__⚠")]
        [Header("⚠__Called when the Quest Dependency is satisfieable and ")]
        [SerializeField]
        private UnityEvent OnSatisfy = new UnityEvent();

        private void OnEnable()
        {
            if (questDependency != null)
                questDependency.OnSatisfied += OnSatisfy.Invoke;
        }

        private void OnDisable()
        {
            if (questDependency != null)
                questDependency.OnSatisfied -= OnSatisfy.Invoke;
        }
    }
}