/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量拆分
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/ToXY")]
    public class Vector2ToXY : ActionNode
    {
        public Ref<Vector2> input;
        public Ref<float> x;
        public Ref<float> y;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { input,x,y};
        }

        protected override Status OnUpdate()
        {
            x.Value = input.Value.x;
            y.Value = input.Value.y;
            return Status.Success;
        }
    }
}
