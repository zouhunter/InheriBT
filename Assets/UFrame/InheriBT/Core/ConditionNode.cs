using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT
{
    /// <summary>
    /// 条件节点 (只返回成功或失败)
    /// </summary>
    public abstract class ConditionNode : BaseNode
    {
        //取反
        public CompareType compareType;

        protected abstract bool CheckCondition();

        protected override Status OnUpdate()
        {
            if(compareType == CompareType.ForceSuccess)
            {
                return Status.Success;
            }
            else if(compareType == CompareType.ForceFailure)
            {
                return Status.Failure;
            }

            if (CheckCondition())
            {
                return compareType == CompareType.Equal ? Status.Success : Status.Failure;
            }
            else
            {
                return compareType == CompareType.NotEqual ? Status.Success : Status.Failure;
            }
        }
    }
}
