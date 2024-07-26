/*-*-* Copyright (c) -your copyright information-
 * Author: -your author information-
 * Creation Date: -today's date-
 * Version: 1.0.0
 * Description: Stores the inverse of the specified quaternion.
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/Inverse")]
    public class QuaternionInverse : ActionNode
    {
        [Tooltip("The target quaternion")]
        public Ref<Quaternion> targetQuaternion;
        [Tooltip("The stored result of the inverse quaternion")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { targetQuaternion, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.Inverse(targetQuaternion.Value);
            return Status.Success;
        }
    }
}
