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
    [NodePath("Vector2/Normalize")]
    public class Vector2Normalize : ActionNode
    {
        public Ref<Vector2> input;
        public Ref<Vector2> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { input, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = input.Value.normalized;
            return Status.Success;
        }
    }
}

