/*-*-* Copyright (c) mateai@wekoi
 * Author: zouhunter
 * Creation Date: 2024-03-18
 * Version: 1.0.0
 * Description: 
 *_*/
using UnityEngine;

namespace UFrame.InheriBT.Composite
{
    [Icon("d32f060aec2e5df4cb1a4af839e5a832")]
    public class Sequence_AllSuccessNode : CompositeNode
    {
        public override MatchType abortType => MatchType.AllSuccess;

        protected override Status OnUpdate()
        {
            if (ChildCount == 0)
                return Status.Success;

            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChild(i);
                var childStatus = child?.Execute() ?? Status.Inactive;
                if(childStatus == Status.Running)
                    return Status.Running;
                else if(childStatus == Status.Failure)
                    return Status.Failure;
            }
            return Status.Success;
        }
    }
}
