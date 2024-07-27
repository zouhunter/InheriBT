using UnityEngine;

namespace UFrame.InheriBT
{
    [DisallowMultipleComponent]
    public abstract class ActionNode : BaseNode
    {
        public override void Dispose()
        {
            //if (TreeInfo != null && TreeInfo.subTrees != null && TreeInfo.subTrees.Count > 0)
            //{
            //    foreach (var subTree in TreeInfo.subTrees)
            //    {
            //        if (subTree.enable)
            //        {
            //            subTree.node.Dispose();
            //        }
            //    }
            //}
            base.Dispose();
        }

        public override Status Execute(TreeInfo info)
        {
            var result = base.Execute(info);
            if(!_conditionFaliure)
            {
                if (info.subTrees != null && info.subTrees.Count > 0)
                {
                    foreach (var subTree in info.subTrees)
                    {
                        if (subTree.enable)
                        {
                            subTree.node.Execute(subTree);
                        }
                    }
                }
            }
            return result;
        }
    }
}
