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
    [NodePath("Vector3/ToXYZ")]
    public class Vector3ToXYZ : ActionNode
    {
        public Ref<Vector3> input;
        public Ref<float> x;
        public Ref<float> y;
        public Ref<float> z;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { input,x,y,z};
        }

        protected override Status OnUpdate()
        {
            x.Value = input.Value.x;
            y.Value = input.Value.y;
            z.Value = input.Value.z;
            return Status.Success;
        }
    }
}
