/*-*-* Copyright (c) -your copyright information-
 * Author: -your author information-
 * Creation Date: -today's date-
 * Version: 1.0.0
 * Description: Stores a rotation which rotates from the first direction to the second.
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/FromToRotation")]
    public class QuaternionFromToRotation : ActionNode
    {
        [Tooltip("The from direction")]
        public Ref<Vector3> fromDirection;
        [Tooltip("The to direction")]
        public Ref<Vector3> toDirection;
        [Tooltip("The stored result")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { fromDirection, toDirection, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.FromToRotation(fromDirection.Value, toDirection.Value);
            return Status.Success;
        }
    }
}
