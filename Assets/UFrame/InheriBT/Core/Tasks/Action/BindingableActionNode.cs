/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-14
 * Version: 1.0.0
 * Description: 通用绑定节点
 *_*/

using UnityEngine.Events;

namespace UFrame.InheriBT.Actions
{
    public abstract class BindingableActionNode : ActionNode
    {
        public UnityEvent<BindingableActionNode> onUpdate;

        protected override Status OnUpdate()
        {
            onUpdate?.Invoke(this);
            return Status;
        }

        public void SetStatus(Status status)
        {
            Status = status;
        }
    }
}

