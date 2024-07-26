using UnityEngine;

namespace UFrame.InheriBT
{
    [DisallowMultipleComponent]
    public abstract class ActionNode : BaseNode
    {
        public override void Dispose()
        {
            if (TreeInfo != null && TreeInfo.subTrees != null && TreeInfo.subTrees.Count > 0)
            {
                foreach (var subTree in TreeInfo.subTrees)
                {
                    if (subTree.enable)
                    {
                        subTree.node.Dispose();
                    }
                }
            }
            base.Dispose();
        }

        public override Status Execute()
        {
            var result = base.Execute();
            if(!_conditionFaliure)
            {
                if (TreeInfo.subTrees != null && TreeInfo.subTrees.Count > 0)
                {
                    foreach (var subTree in TreeInfo.subTrees)
                    {
                        if (subTree.enable)
                        {
                            subTree.node.Execute();
                        }
                    }
                }
            }
            return result;
        }
    }
}
