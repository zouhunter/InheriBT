/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 四元数角度
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/Angle")]
    public class QuaternionAngle : ActionNode
    {
        public Ref<Quaternion> firstRotation;
        public Ref<Quaternion> secondRotation;
        public Ref<float> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { firstRotation, secondRotation, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.Angle(firstRotation.Value, secondRotation.Value);
            return Status.Success;
        }
    }
}
