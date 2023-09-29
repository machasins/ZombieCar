using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Progression {
    /// <summary>
    /// A flag that can be set by referencing this asset at runtime. Useful for configuring progression point dependencies.
    /// </summary>
    [CreateAssetMenu(fileName = "Has_Something", menuName = "Progression/Flag")]
    public class FlagDependency : Dependency
    {
        [SerializeField]
        private bool flag;

        public bool Flag { get { return flag; } set { flag = value; } }

        /// <summary>
        /// Sets the flag to true and forces the OnSatisfy event to call.
        /// Any progression points dependend on this flag will still check against all other dependencies.
        /// </summary>
        public void ForceSatisfy()
        {
            flag = true;
            Satisfy();
        }

        /// <summary>
        /// If the flag is true or not.
        /// </summary>
        /// <returns></returns>
        public override bool Satisfied()
        {
            return Flag;
        }
    }
}