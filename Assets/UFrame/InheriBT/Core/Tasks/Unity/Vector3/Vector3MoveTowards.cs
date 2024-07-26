/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 位置移动
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/MoveTowards")]
    public class Vector3MoveTowards : ActionNode
    {
        public Ref<Vector3> pos;
        public Ref<Vector3> targetPos;
        public Ref<float> speed;
        public Ref<Vector3> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { pos, targetPos, speed, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = Vector3.MoveTowards(pos.Value, targetPos.Value, speed.Value * Time.deltaTime);
            return Status.Success;
        }
    }
}

