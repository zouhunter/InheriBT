/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量倍数
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/Multiply")]
    public class Vector3Multiply : ActionNode
    {
        public Ref<Vector3> input;
        public Ref<float> multiplyBy;
        public Ref<Vector3> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { input, multiplyBy, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = input.Value * multiplyBy.Value;
            return Status.Success;
        }
    }
}

