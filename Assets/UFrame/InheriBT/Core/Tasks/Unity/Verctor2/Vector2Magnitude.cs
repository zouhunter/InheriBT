/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量长度
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/Magnitude")]
    public class Vector2Magnitude : ActionNode
    {
        public Ref<Vector2> input;
        public Ref<float> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { input, result };
        }
      
        protected override Status OnUpdate()
        {
            result.Value = input.Value.magnitude;
            return Status.Success;
        }
    }
}
