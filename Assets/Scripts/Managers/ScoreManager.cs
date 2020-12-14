using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MiniAstro.Environment;

namespace MiniAstro.Management
{
    public class ScoreManager : MonoBehaviour
    {
        public static ScoreManager instance;

        [Header("Resources to gather")]
        public int hematiteToGather = 100;
        int hematiteGathered = 0;
        public int malachiteToGather = 100;
        int malachiteGathered = 0;
        public int quartzToGather = 100;
        int quartzGathered = 0;

        [Header("Resources panel")]
        public ResourcesPanel resourcesPanel;

        public bool LevelFinished
        {
            get
            {
                return (hematiteGathered >= hematiteToGather && malachiteGathered >= malachiteToGather && quartzGathered >= quartzToGather);
            }
        }

        void Awake()
        {
            instance = this;
        }

        public void Add(MineralOreType oreType, int amount)
        {
            switch (oreType)
            {
                case MineralOreType.Hematite:
                    hematiteGathered += amount;
                    break;
                case MineralOreType.Malachite:
                    malachiteGathered += amount;
                    break;
                case MineralOreType.Quartz:
                    quartzGathered += amount;
                    break;
                default:
                    break;
            }
            resourcesPanel.AddResource((int)oreType, amount);

        }

    }

}

