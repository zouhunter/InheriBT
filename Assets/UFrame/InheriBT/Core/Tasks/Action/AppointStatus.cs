/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-29
 * Version: 1.0.0
 * Description: Returns a Status of appoint.
 *_*/

using UFrame.InheriBT;
using UFrame;

namespace UFrame.InheriBT.Actions
{
    [NodePath("AppointStatus")]
    public class AppointStatus : ActionNode
    {
        public Ref<Status> status;
        protected override Status OnUpdate()
        {
            return status;
        }
    }
}
