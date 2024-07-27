/*-*-* Copyright (c) UFrame@wekoi
 * Author: zouhunter
 * Creation Date: 2024-03-18
 * Version: 1.0.0
 * Description: 
 *_*/
using System;

using UnityEngine;

namespace UFrame.InheriBT.Composite
{
    [Icon("d32f060aec2e5df4cb1a4af839e5a832")]
    [Serializable]
    public class Sequence_AnySuccessNode : CompositeNode
    {
        protected override Status OnUpdate(TreeInfo info)
        {
            if (GetChildCount(info) == 0)
                return Status.Inactive;

            for (int i = 0; i < GetChildCount(info); i++)
            {
                var child = GetChild(info,i);
                var childStatus = child.node?.Execute(child) ?? Status.Inactive;
                if (childStatus == Status.Running)
                    return Status.Running;
                if(childStatus == Status.Success)
                    return Status.Success;
            }
            return Status.Failure;
        }
    }
}
