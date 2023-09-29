using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Progression
{
    [CreateAssetMenu(fileName = "Not_Something", menuName = "Progression/Not")]
    public class NotDependency : Dependency
    {
        [SerializeField]
        private Dependency dependency = null;

        private bool circularDependencyCheck = false;

        public override bool Satisfied()
        {
            if (circularDependencyCheck)
            {
                if (dependency == this)
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is dependent on itself for completion this is likely unintended and will be skipped over. Please remove " + name + " from itself.", this);
                else
                {
                    System.Type type = new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType;
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is indirectly dependent on itself through a " + type.Name + " this is likely unintended and will be skipped over.", this);
                }
                return true;
            }

            circularDependencyCheck = true;
            bool satisfied = dependency == null? true : !dependency.Satisfied();
            circularDependencyCheck = false;
            return satisfied;
        }

        internal override void Save()
        {
            if (circularDependencyCheck)
            {
                if (dependency == this)
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is dependent on itself for completion this is likely unintended and will be skipped over. Please remove " + name + " from the dependencies.", this);
                else
                {
                    System.Type type = new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType;
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is indirectly dependent on itself through a " + type.Name + " this is likely unintended and will be skipped over.", this);
                }
                return;
            }
            circularDependencyCheck = true;
            try
            {
                if(dependency != null)
                    dependency.Save();
            }
            finally
            {
                circularDependencyCheck = false;
            }
            circularDependencyCheck = false;
            base.Save();
        }

        internal override void Load()
        {
            if (circularDependencyCheck)
            {
                if (dependency == this)
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is dependent on itself for completion this is likely unintended and will be skipped over. Please remove " + name + " from the dependencies.", this);
                else
                {
                    System.Type type = new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType;
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is indirectly dependent on itself through a " + type.Name + " this is likely unintended and will be skipped over.", this);
                }
                return;
            }
            circularDependencyCheck = true;
            try
            {
                if(dependency != null)
                    dependency.Load();
            }
            finally
            {
                circularDependencyCheck = false;
            }
            circularDependencyCheck = false;
            base.Load();
        }
    }
}