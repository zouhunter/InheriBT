/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量组合
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/FromXYZ")]
    public class Vector3FromXYZ : ActionNode
    {
        public Ref<float> x;
        public Ref<float> y;
        public Ref<float> z;
        public Ref<Vector3> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { x, y, z,result };
        }

        protected override Status OnUpdate()
        {
            result.Value = new Vector3(x.Value, y.Value, z.Value);
            return Status.Success;
        }
    }
}
