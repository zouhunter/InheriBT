/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量旋转
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/RotateTowards")]
    public class Vector3RotateTowards : ActionNode
    {
        public Ref<Vector3> current;
        public Ref<Vector3> target;
        public Ref<float> degreesDelta;
        public Ref<float> magnitudeDelta;
        public Ref<Vector3> result;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { current, target, degreesDelta, magnitudeDelta, result };
        }

        protected override Status OnUpdate()
        {
            result.Value = Vector3.RotateTowards(current.Value, target.Value, degreesDelta.Value * Mathf.Deg2Rad * Time.deltaTime, magnitudeDelta.Value);
            return Status.Success;
        }
    }
}

