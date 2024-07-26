/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-12
 * Version: 1.0.0
 * Description: 不论对错，都返回成功
 *_*/

using UnityEngine;

namespace UFrame.InheriBT.Decorates
{
    [AddComponentMenu("BehaviourTree/Decorate/LikeSuccess")]
    public class LikeSuccess : DecorateNode
    {
        protected override Status OnUpdate()
        {
            var status = ExecuteChild();
            if(status == Status.Failure)
            {
                return Status.Success;
            }
            return status;
        }
    }
}
