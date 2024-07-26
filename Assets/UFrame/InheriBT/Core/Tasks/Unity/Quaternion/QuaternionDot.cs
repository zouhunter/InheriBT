/*-*-* Copyright (c) -your copyright information-
 * Author: -your author information-
 * Creation Date: -today's date-
 * Version: 1.0.0
 * Description: Stores the dot product between two rotations.
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/Dot")]
    public class QuaternionDot : ActionNode
    {
        [Tooltip("The first rotation")]
        public Ref<Quaternion> leftRotation;
        [Tooltip("The second rotation")]
        public Ref<Quaternion> rightRotation;
        [Tooltip("The stored result")]
        public Ref<float> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { leftRotation, rightRotation, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.Dot(leftRotation.Value, rightRotation.Value);
            return Status.Success;
        }
    }
}
