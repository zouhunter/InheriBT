/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 线性插值
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/Lerp")]
    public class Vector2Lerp : ActionNode
    {
        public Ref<Vector2> inputA;
        public Ref<Vector2> inputB;
        public Ref<float> retio;
        public Ref<Vector2> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { inputA, inputB, retio, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = Vector2.Lerp(inputA.Value,inputB.Value,retio.Value);
            return Status.Success;
        }
    }
}
