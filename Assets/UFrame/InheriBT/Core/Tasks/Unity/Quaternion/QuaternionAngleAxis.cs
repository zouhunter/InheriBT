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
    [NodePath("Quaternion/AngleAxis")]
    public class AngleAxis : ActionNode
    {
        [Tooltip("The number of degrees")]
        public Ref<float> degrees;
        [Tooltip("The axis direction")]
        public Ref<Vector3> axis;
        [Tooltip("The stored result")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { degrees, axis, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.AngleAxis(degrees.Value, axis.Value);
            return Status.Success;
        }
    }
}
