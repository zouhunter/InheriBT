/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 3d向量转2d向量
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/ToVector3")]
    public class Vector2ToVector2 : ActionNode
    {
        public Ref<Vector2> input;
        public Ref<Vector3> result;


        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { input, result };
        }


        protected override Status OnUpdate()
        {
            result.Value = input.Value;
            return Status.Success;
        }
    }
}
