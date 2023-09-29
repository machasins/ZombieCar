
using UnityEngine;
using System.Linq;

namespace Progression
{
    // Quest lines are connected through prerequisites.
    // Multiple Prereqs can be used and multiple sets of prereqs can be used
    // No questline can be mandatory prereq for one of it's mandatory prereqs
    [CreateAssetMenu(fileName = "Quests", menuName = "Progression/QuestManager", order = -10000)]
    public sealed class QuestManagerAsset : BaseScriptableObject
    {
        [SerializeField]
        internal QuestAsset[] quests = new QuestAsset[0];

        [SerializeField]
        internal ProgressionPointAsset[] progressionPoints = new ProgressionPointAsset[0];

        [SerializeField]
        internal Dependency[] dependencies = new Dependency[0];

        /// <summary>
        /// Get all quests.
        /// </summary>
        /// <returns></returns>
        public QuestAsset[] GetQuests()
        {
            return (QuestAsset[])quests.Clone();
        }

        /// <summary>
        /// Get all progression points.
        /// </summary>
        /// <returns></returns>
        public ProgressionPointAsset[] GetProgressionPoints()
        {
            return (ProgressionPointAsset[])progressionPoints.Clone();
        }
         
        /// <summary>
        /// Get all dependencies in use by progression points.
        /// Note: will not return children of dependencies.
        /// </summary>
        /// <returns></returns>
        public Dependency[] GetDependencies()
        {
            return GetProgressionPoints().SelectMany(x => x.dependencies).ToArray();
        }

        /// <summary>
        /// Get all quests that have at least one completed progress point, but also have not completed.
        /// </summary>
        /// <returns></returns>
        public QuestAsset[] GetActiveQuests()
        {
            return quests.Where(x => { float p = x.Progress; return p > 0.0f && p < 1.0f; }).ToArray();
        }

        /// <summary>
        /// Get all quests that have all progression points completed.
        /// </summary>
        /// <returns></returns>
        public QuestAsset[] GetCompletedQuests()
        {
            return quests.Where(x => x.Completed).ToArray();
        }

        internal override void Save()
        {
            for(int i = 0; i < quests.Length; i++)
            {
                if (quests[i] == null)
                    continue;
                quests[i].saveSlot = saveSlot;
                quests[i].Save();
            }
            base.Save();
        }

        internal override void Load()
        {
            for (int i = 0; i < quests.Length; i++)
            {
                if (quests[i] == null)
                    continue;
                quests[i].saveSlot = saveSlot;
                quests[i].Load();
            }
            base.Load();
        }
    }
}