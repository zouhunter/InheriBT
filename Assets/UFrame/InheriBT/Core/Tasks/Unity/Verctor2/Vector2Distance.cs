/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 判断坐标距离
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/Distance")]
    public class Vector2Distance : ActionNode
    {
        public Ref<Vector2> inputA;
        public Ref<Vector2> inputB;
        public Ref<float> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { inputA, inputB, result };
        }
 
        protected override Status OnUpdate()
        {
            result.Value = Vector2.Distance(inputA.Value, inputB.Value);
            return Status.Success;
        }
    }
}
