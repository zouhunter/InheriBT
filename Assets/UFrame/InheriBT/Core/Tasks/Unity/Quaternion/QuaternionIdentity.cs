/*-*-* Copyright (c) -your copyright information-
 * Author: -your author information-
 * Creation Date: -today's date-
 * Version: 1.0.0
 * Description: Stores the quaternion identity.
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/Identity")]
    public class QuaternionIdentity : ActionNode
    {
        [Tooltip("The stored identity quaternion result")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.identity;
            return Status.Success;
        }
    }
}
