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
    [NodePath("Vector3/Lerp")]
    public class Vector3Lerp : ActionNode
    {
        public Ref<Vector3> inputA;
        public Ref<Vector3> inputB;
        public Ref<float> retio;
        public Ref<Vector3> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { inputA, inputB, retio, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = Vector3.Lerp(inputA.Value,inputB.Value,retio.Value);
            return Status.Success;
        }
    }
}
