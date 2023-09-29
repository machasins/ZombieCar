using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
namespace Progression
{
    public enum GroupType { AllSatisfied, AnySatisfied}
    /// <summary>
    /// Have a group of dependencies that can be relient on all or any of them to be satisfied.
    /// </summary>
    [CreateAssetMenu(fileName = "Has_Somethings", menuName = "Progression/Group")]
    public class DependencyGroup : Dependency
    {
        [SerializeField]
        private GroupType require = GroupType.AllSatisfied;

        [SerializeField]
        private Dependency[] dependencies = new Dependency[0];

        private bool circularDependencyCheck = false;

        public override bool Satisfied()
        {
            if (circularDependencyCheck)
            {
                if (dependencies.Contains(this))
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is dependent on itself for completion this is likely unintended and will be skipped over. Please remove " + name+ " from the dependencies.", this);
                else
                {
                    System.Type type = new System.Diagnostics.StackFrame(1).GetMethod().DeclaringType;
                    Debug.LogWarning("Circular Dependency Detected: " + name + " is indirectly dependent on itself through a " + type.Name + " this is likely unintended and will be skipped over.", this);
                }
                return true;
            }
            circularDependencyCheck = true;
            bool satisfied = true;
            try
            {
                switch (require)
                {
                    case GroupType.AllSatisfied:
                        satisfied = dependencies.All(x => { return x == null ? true : x.Satisfied(); });
                        break;
                    case GroupType.AnySatisfied:
                        satisfied = dependencies.Any(x => { return x == null ? true : x.Satisfied(); });
                        break;
                }
            }
            finally
            {
                // needs to be set to false in this finally, otherwise next time we run this it will think it is in a circular dependency when it is not.
                circularDependencyCheck = false;
            }

            circularDependencyCheck = false;
            return satisfied;
        }

        internal override void Save()
        {
            if (circularDependencyCheck)
            {
                if (dependencies.Contains(this))
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
                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (dependencies[i] == null)
                        continue;
                    dependencies[i].saveSlot = saveSlot;
                    dependencies[i].Save();
                }
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
                if (dependencies.Contains(this))
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
                for (int i = 0; i < dependencies.Length; i++)
                {
                    if (dependencies[i] == null)
                        continue;
                    dependencies[i].saveSlot = saveSlot;
                    dependencies[i].Load();
                }
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