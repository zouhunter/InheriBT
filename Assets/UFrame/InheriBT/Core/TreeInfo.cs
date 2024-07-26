using System;
using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT
{
    [System.Serializable]
    public class TreeInfo
    {
        public string id;
        public bool enable;
        public string desc;
        [SerializeReference]
        public BaseNode node;
        public ConditionInfo condition = new ConditionInfo();
        [SerializeReference]
        public List<TreeInfo> subTrees;
    }

    [Serializable]
    public class ConditionInfo
    {
        public bool enable;
        public MatchType matchType = MatchType.AllSuccess;
        public List<ConditionItem> conditions = new List<ConditionItem>();
    }

    [Serializable]
    public class ConditionItem
    {
        public MatchType matchType = MatchType.AllSuccess;
        [SerializeReference]
        public ConditionNode node;
        public bool subEnable;
        public List<SubConditionItem> subConditions = new List<SubConditionItem>();
    }

    [Serializable]
    public class SubConditionItem
    {
        [SerializeReference]
        public ConditionNode node;
    }
}
