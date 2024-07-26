/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-12
 * Version: 1.0.0
 * Description: 不论对错，都返回失败
 *_*/

using UnityEngine;

namespace UFrame.InheriBT.Decorates
{
    [AddComponentMenu("BehaviourTree/Decorate/LikeFailure")]
    public class LikeFailure : DecorateNode
    {
        protected override Status OnUpdate()
        {
            var status = base.ExecuteChild();
            if(status == Status.Success)
            {
                return Status.Failure;
            }
            return status;
        }
    }
}
