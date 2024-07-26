/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-18
 * Version: 1.0.0
 * Description: 装饰器
 *_*/

namespace UFrame.InheriBT
{
    /// <summary>
    /// 装饰器
    /// </summary>
    public abstract class DecorateNode : ParentNode
    {
        public override int maxChildCount => 1;

        protected virtual Status ExecuteChild()
        {
            var childNode = GetChild(0);
            if (childNode != null)
            {
               return childNode.Execute();
            }
            return Status.Inactive;
        }
    }
}
