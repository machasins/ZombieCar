using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
namespace Progression
{

    internal sealed class ProgressionPointEvent : UnityEvent<ProgressionPointAsset> { }
    /// <summary>
    /// A specific point in a questline that is accomplished when all the dependencies are satisfied.
    /// Note: Progression points are not optional for completing a quest in which they are part of.
    /// </summary>
    [System.Serializable]
    public sealed class ProgressionPointAsset : BaseScriptableObject
    {
        [SerializeField]
        internal UnityEvent OnCompleted = null;

        [SerializeField]
        internal List<Dependency> dependencies = new List<Dependency>();

        internal Func<ProgressionPointAsset, bool> OnCompletedThis;

        /// <summary>
        /// Whether this progression point is completed in it's questline.
        /// </summary>
        /// <returns></returns>
        public bool IsComplete()
        {
            if (OnCompletedThis == null)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// In other words, are all the dependencies satisfied.
        /// May not be completed if it is not the next sequential progression point.
        /// </summary>
        /// <returns></returns>
        public bool AreDependenciesSatisfied()
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i] == null)
                    continue;
                if (!dependencies[i].Satisfied())
                    return false;
            }
            return true;
        }

        /// <summary>
        /// Attempt to complete this progression point.
        /// Can only be completed if all dependencies are satisfied and it is the next progression point in the quest.
        /// </summary>
        public void Complete()
        {
            SatisfiedADependency();
        }

        internal override void Save()
        {
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i] == null)
                    continue;
                dependencies[i].saveSlot = saveSlot;
                dependencies[i].Save();
            }
            base.Save();
        }

        internal override void Load()
        {
            OnDisable();
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i] == null)
                    continue;
                dependencies[i].saveSlot = saveSlot;
                dependencies[i].Load();
            }
            base.Load();
            OnEnable();
        }

        internal protected override void OnEnable()
        {
            base.OnEnable();
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i] == null)
                    continue;
                dependencies[i].OnSatisfied += SatisfiedADependency;
            }
        }

        internal protected override void OnDisable()
        {
            base.OnDisable();
            for (int i = 0; i < dependencies.Count; i++)
            {
                if (dependencies[i] == null)
                    continue;
                dependencies[i].OnSatisfied -= SatisfiedADependency;
            }
        }

        private void SatisfiedADependency()
        {
            if (AreDependenciesSatisfied())
                if(OnCompletedThis != null)
                    if (OnCompletedThis.Invoke(this))
                        if(OnCompleted != null)
                            OnCompleted.Invoke();
        }
    }
}