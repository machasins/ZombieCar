using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;
namespace Progression
{
    /// <summary>
    /// Representation of a quest arc. This could be a sub arc, the main arc, etc.
    /// </summary>
    [System.Serializable]
    public sealed class QuestAsset : BaseScriptableObject
    {
        #region fields

        internal List<ProgressionPointAsset> completedPoints = new List<ProgressionPointAsset>();

        [SerializeField]
        internal List<ProgressionPointAsset> progressionPoints = new List<ProgressionPointAsset>();

        [SerializeField]
        [HideInInspector]
        private int lastCompleted = -1;

        #endregion
        #region properties

        /// <summary>
        /// The completion status of this questline.
        /// </summary>
        public bool Completed { get { return completedPoints.Count == progressionPoints.Count; } }

        /// <summary>
        /// A decimal representation of how many of the progression points have been completed.
        /// </summary>
        public float Progress { get { return ((float)completedPoints.Count / (float)progressionPoints.Count); } }

        /// <summary>
        /// The next incomplete progression point in the questline.
        /// </summary>
        public ProgressionPointAsset NextProgressionPoint
        {
            get
            {
                if (IndexOfNextProgressionPointToComplete >= 0 && IndexOfNextProgressionPointToComplete <= progressionPoints.Count - 1)
                    return progressionPoints[IndexOfNextProgressionPointToComplete];
                else return null;
            }
        }

        private int IndexOfNextProgressionPointToComplete { get { return completedPoints.Count; } }

        #endregion
        #region functions

        /// <summary>
        /// Returns an array of all the progression points in this questline.
        /// </summary>
        /// <returns></returns>
        public ProgressionPointAsset[] GetProgressionPoints()
        {
            return progressionPoints.ToArray();
        }

        /// <summary>
        /// Returns an array of all the completed progression points in this questline.
        /// </summary>
        /// <returns></returns>
        public ProgressionPointAsset[] GetCompletedProgressionPoints()
        {
            return completedPoints.ToArray();
        }

        internal override void Load()
        {
            OnDisable();
            for (int i = 0; i < progressionPoints.Count; i++)
            {
                if (progressionPoints[i] == null)
                    continue;
                progressionPoints[i].saveSlot = saveSlot;
                progressionPoints[i].Load();
            }
            base.Load();
            completedPoints.Clear();
            for(int i = lastCompleted; i >=0; i--)
                completedPoints.Add(progressionPoints[i]);
            OnEnable();
        }

        internal override void Save()
        {
            for (int i = 0; i < progressionPoints.Count; i++)
            {
                if (progressionPoints[i] == null)
                    continue;
                progressionPoints[i].saveSlot = saveSlot;
                progressionPoints[i].Save();
            }
            lastCompleted = IndexOfNextProgressionPointToComplete - 1;
            base.Save();
        }

        internal protected override void OnEnable()
        {
            base.OnEnable();
            for (int i = IndexOfNextProgressionPointToComplete; i < progressionPoints.Count; i++)
                progressionPoints[i].OnCompletedThis += CompletedProgressionPoint;
        }

        internal protected override void OnDisable()
        {
            base.OnDisable();
            for (int i = IndexOfNextProgressionPointToComplete; i < progressionPoints.Count; i++)
                progressionPoints[i].OnCompletedThis -= CompletedProgressionPoint;
        }

        private bool CompletedProgressionPoint(ProgressionPointAsset progressionPoint)
        {
            if (NextProgressionPoint == progressionPoint)
            {
                completedPoints.Add(progressionPoint);
                progressionPoint.OnCompletedThis -= CompletedProgressionPoint;
                return true;
            }
            else if (completedPoints.Contains(progressionPoint))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}