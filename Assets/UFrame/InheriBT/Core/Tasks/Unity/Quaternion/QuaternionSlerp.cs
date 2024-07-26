/*-*-*-*-*-*
 * Author: zouhunter
 * Creation Date: 2024-03-29
 * Description: Spherically lerp between two quaternions.
 *-*-*-*-*-*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Quaternion/Slerp")]
    public class QuaternionSlerp : ActionNode
    {
        [Tooltip("The from rotation.")]
        public Ref<Quaternion> fromQuaternion;
        [Tooltip("The to rotation.")]
        public Ref<Quaternion> toQuaternion;
        [Tooltip("The amount to lerp.")]
        public Ref<float> amount;
        [Tooltip("The stored result.")]
        public Ref<Quaternion> storeResult;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { fromQuaternion, toQuaternion, amount, storeResult };
        }

        protected override Status OnUpdate()
        {
            storeResult.Value = Quaternion.Slerp(fromQuaternion.Value, toQuaternion.Value, amount.Value);
            return Status.Success;
        }
    }
}
