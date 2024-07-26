/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-12
 * Version: 1.0.0
 * Description: 直到子节点返回成功
 *_*/

using UnityEngine;

namespace UFrame.InheriBT.Decorates
{
    [AddComponentMenu("BehaviourTree/Decorate/UntilSuccess")]
    public class UntilSuccess : DecorateNode
    {
        protected override Status OnUpdate()
        {
            var childResult = base.ExecuteChild();
            if (childResult == Status.Success)
            {
                return Status.Success;
            }
            return Status.Running;
        }
    }
}
