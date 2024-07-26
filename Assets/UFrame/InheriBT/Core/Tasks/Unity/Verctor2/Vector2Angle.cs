/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 计算3D向量夹角
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/Angle")]
    public class Vector2Angle : ActionNode
    {
        public Ref<Vector2> inputA;
        public Ref<Vector2> inputB;
        public Ref<float> resultAngle;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { inputA, inputB, resultAngle };
        }

        protected override Status OnUpdate()
        {
            resultAngle.Value = Vector2.Angle(inputA.Value, inputB.Value);
            return Status.Success;
        }
    }
}

