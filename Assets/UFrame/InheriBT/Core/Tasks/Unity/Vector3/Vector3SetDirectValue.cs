/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 设置指定的Vector3变量为指定的值
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/SetDirectValue")]
    public class Vector3SetDirectValue : ActionNode
    {
        public Ref<Vector3> target;
        public Vector3 value;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { target };
        }

        protected override Status OnUpdate()
        {
            target.Value = value;
            return Status.Success;
        }
    }
}
