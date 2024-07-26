/*-*-* Copyright (c) Your Copyright Information
 * Author: zouhunter
 * Creation Date: 2024-03-29
 * Version: 1.0.0
 * Description: Lerps between two quaternions.
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/Lerp")]
    public class QuaternionLerp : ActionNode
    {
        [Tooltip("The from rotation")]
        public Ref<Quaternion> fromQuaternion;
        [Tooltip("The to rotation")]
        public Ref<Quaternion> toQuaternion;
        [Tooltip("The amount to lerp")]
        public Ref<float> amount;
        [Tooltip("The stored result")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { fromQuaternion, toQuaternion, amount, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.Lerp(fromQuaternion.Value, toQuaternion.Value, amount.Value);
            return Status.Success;
        }
    }
}
