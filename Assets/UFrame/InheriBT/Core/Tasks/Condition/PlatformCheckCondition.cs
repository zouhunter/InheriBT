/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-18
 * Version: 1.0.0
 * Description: 
 *_*/

using UnityEngine;

namespace UFrame.InheriBT.Condition
{
    public class PlatformCheckCondition : ConditionNode
    {
        public RuntimePlatform platform;
        protected override bool CheckCondition()
        {
            return Application.platform == platform;
        }
    }
}
