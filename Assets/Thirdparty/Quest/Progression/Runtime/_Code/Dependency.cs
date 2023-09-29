using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Progression
{
    public interface IDependency
    {
        bool Satisfied();
    }

    /// <summary>
    /// An abstraction of a requirement to get to a point (progression point) in the quest.
    /// Note: Dependencies points can be optional for completing progress points.
    /// </summary>
    public abstract class Dependency : BaseScriptableObject, IDependency
    {

        internal Action OnSatisfied;

        /// <summary>
        /// If the dependency is satisfied, then alerts ProgressionPoints that the dependency has been satisfied.
        /// Returns false if the dependency is not satisfied.
        /// </summary>
        /// <returns></returns>
        public bool Satisfy()
        {
            if (Satisfied())
            {
                if(OnSatisfied != null)
                    OnSatisfied.Invoke();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Identical to Satisfy except has no return type (Allows it to be called through UnityEvents).
        /// </summary>
        public void SatisfyIfAble()
        {
            Satisfy();
        }

        /// <summary>
        /// Checks if the dependency is satisfied.
        /// </summary>
        /// <returns></returns>
        public abstract bool Satisfied();

        /// <summary>
        /// Called after the Asset is generated/reimported.
        /// </summary>
        protected override void OnAssetGenerated()
        {
            Satisfied();
        }

        internal protected override void OnEnable()
        {
            base.OnEnable();
            Satisfied();
        }
    }
}