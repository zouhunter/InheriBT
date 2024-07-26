/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量点乘
 *_*/

using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector2/SyncValue")]
    public class Vector2SyncValue : ActionNode
    {
        public Ref<Vector2> source;
        public Ref<Vector2> target;

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { source, target };
        }

        protected override Status OnUpdate()
        {
            target.Value = source.Value;
            return Status.Success;
        }
    }
}
