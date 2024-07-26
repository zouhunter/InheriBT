using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT
{
    public class CopyPasteUtil
    {
        public static BaseNode copyNode;
        public static TreeInfo copyedTreeInfo;

        public static void CopyTreeInfo(TreeInfo source, TreeInfo target, TreeInfo rootTarget)
        {
            target.node = source.node;
            target.enable = source.enable;
            target.condition = new ConditionInfo();
            target.condition.enable = source.condition.enable;
            target.condition.conditions = new List<ConditionItem>();
            target.condition.matchType = source.condition.matchType;
            if (source.condition.conditions != null)
            {
                foreach (var item in source.condition.conditions)
                {
                    var conditionItem = new ConditionItem();
                    conditionItem.node = item.node;
                    conditionItem.subEnable = item.subEnable;
                    conditionItem.matchType = item.matchType;
                    if (item.subConditions != null)
                        conditionItem.subConditions = new List<SubConditionItem>(item.subConditions);
                    target.condition.conditions.Add(conditionItem);
                }
            }
            if (source.subTrees != null)
            {
                target.subTrees = new List<TreeInfo>();
                foreach (var item in source.subTrees)
                {
                    if (item == rootTarget)
                        continue;

                    var subTree = new TreeInfo();
                    CopyTreeInfo(item, subTree, rootTarget);
                    target.subTrees.Add(subTree);
                }
            }
        }
    }
}
