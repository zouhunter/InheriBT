/*-*-*-*-*-*
 * Author: zouhunter
 * Creation Date: 2024-03-29
 * Description: Stores the quaternion after a rotation towards another quaternion given a maximum degrees delta.
 *-*-*-*-*-*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/RotateTowards")]
    public class QuaternionRotateTowards : ActionNode
    {
        [Tooltip("The from rotation.")]
        public Ref<Quaternion> fromQuaternion;
        [Tooltip("The to rotation.")]
        public Ref<Quaternion> toQuaternion;
        [Tooltip("The maximum degrees delta.")]
        public Ref<float> maxDeltaDegrees;
        [Tooltip("The stored result.")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { fromQuaternion, toQuaternion, maxDeltaDegrees, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.RotateTowards(fromQuaternion.Value, toQuaternion.Value, maxDeltaDegrees.Value);
            return Status.Success;
        }
    }
}
