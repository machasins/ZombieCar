using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Progression
{
    /// <summary>
    /// If the prereq progress is equal to or greater than the minimum progress, then this is satisfied.
    /// questline.progress >= minimumProgress
    /// </summary>
    [CreateAssetMenu(fileName = "Done_Something", menuName = "Progression/Completed")]
    public class ProgressDependency : Dependency
    {
        [SerializeField]
        private ProgressionPointAsset minimumProgress = null;

        public override bool Satisfied()
        {
            return minimumProgress == null? true : minimumProgress.IsComplete();
        }
    }

}