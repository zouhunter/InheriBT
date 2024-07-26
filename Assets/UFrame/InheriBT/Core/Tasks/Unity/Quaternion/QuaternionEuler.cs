/*-*-* Copyright (c) -your copyright information-
 * Author: -your author information-
 * Creation Date: -today's date-
 * Version: 1.0.0
 * Description: Stores the quaternion of a euler vector.
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/Euler")]
    public class QuaternionEuler : ActionNode
    {
        [Tooltip("The euler vector")]
        public Ref<Vector3> eulerVector;
        [Tooltip("The stored quaternion")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { eulerVector, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.Euler(eulerVector.Value);
            return Status.Success;
        }
    }
}
