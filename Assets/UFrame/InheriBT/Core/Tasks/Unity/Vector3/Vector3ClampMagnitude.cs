/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 限定向量长度
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/ClampMagnitude")]
    public class Vector3ClampMagnitude : ActionNode
    {
        public Ref<Vector3> inputA;
        public Ref<float> inputB;
        public Ref<Vector3> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new List<IRef> { inputA, inputB, result };
        }
        protected override Status OnUpdate()
        {
            result.Value = Vector3.ClampMagnitude(inputA.Value, inputB.Value);
            return Status.Success;
        }
    }
}
