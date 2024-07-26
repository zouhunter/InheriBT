/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量点乘
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/Dot")]
    public class Vector3Dot : ActionNode
    {
        public Ref<Vector3> inputA;
        public Ref<Vector3> inputB;
        public Ref<float> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { inputA, inputB, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = Vector3.Dot(inputA.Value, inputB.Value);
            return Status.Success;
        }
    }
}
