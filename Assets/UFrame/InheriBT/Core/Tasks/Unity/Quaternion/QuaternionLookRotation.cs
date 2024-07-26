/*-*-*
 * Author: zouhunter
 * Creation Date: 2024-03-29
 * Description: Stores the quaternion of a forward vector.
 *-*-*-*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/LookRotation")]
    public class QuaternionLookRotation : ActionNode
    {
        [Tooltip("The forward vector")]
        public Ref<Vector3> forwardVector;
        [Tooltip("The up vector (optional)")]
        public Ref<Vector3> secondVector3;
        [Tooltip("The stored quaternion")]
        public Ref<Quaternion> storeResult;


        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { forwardVector, secondVector3, storeResult };
        }

        protected override Status OnUpdate()
        {
            // If 'secondVector3' has a default value, use it as the up-vector. Otherwise, use Vector3.up.
            if (secondVector3.Value != default(Vector3))
            {
                storeResult.Value = Quaternion.LookRotation(forwardVector.Value, secondVector3.Value);
            }
            else
            {
                storeResult.Value = Quaternion.LookRotation(forwardVector.Value);
            }

            return Status.Success;
        }
    }
}
