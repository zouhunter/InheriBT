/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量组合
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/FromXY")]
    public class Vector2FromXY : ActionNode
    {
        public Ref<float> x;
        public Ref<float> y;
        public Ref<Vector2> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { x, y, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = new Vector2(x.Value, y.Value);
            return Status.Success;
        }
    }
}
