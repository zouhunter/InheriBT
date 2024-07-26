/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-18
 * Version: 1.0.0
 * Description: 循序执行节点
 *_*/

using UnityEngine;

namespace UFrame.InheriBT.Composite
{
    [Icon("d32f060aec2e5df4cb1a4af839e5a832")]
    public class SequenceNode : CompositeNode
    {
        [SerializeField]
        private MatchType _abortType;
        public override MatchType abortType => _abortType;

        protected override Status OnUpdate()
        {
            if (ChildCount == 0)
                return Status.Success;

            switch (abortType)
            {
                case MatchType.AllSuccess:
                    return CheckAllSuccess();
                case MatchType.AllFailure:
                    return CheckAllFailure();
                case MatchType.AnySuccess:
                    return CheckAnySuccess();
                case MatchType.AnyFailure:
                    return CheckAnyFailure();
            }
            return Status.Failure;
        }

        /// <summary>
        /// 检查全部成功
        /// </summary>
        /// <returns></returns>
        private Status CheckAllSuccess()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChild(i);
                var childStatus = child?.Execute() ?? Status.Inactive;
                if (childStatus == Status.Running)
                    return Status.Running;
                if(childStatus == Status.Failure)
                    return Status.Failure;
            }
            return Status.Success;
        }
        /// <summary>
        /// 检查任意成功
        /// </summary>
        /// <returns></returns>
        private Status CheckAnySuccess()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChild(i);
                var childStatus = child?.Execute() ?? Status.Inactive;
                if (childStatus == Status.Running)
                    return Status.Running;
                if (childStatus == Status.Success)
                    return Status.Success;
            }
            return Status.Failure;
        }
        /// <summary>
        /// 检查全部失败
        /// </summary>
        /// <returns></returns>
        private Status CheckAllFailure()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChild(i);
                var childStatus = child?.Execute() ?? Status.Inactive;
                if (childStatus == Status.Running)
                    return Status.Running;
                if (childStatus == Status.Success)
                    return Status.Failure;
            }
            return Status.Success;
        }
        /// <summary>
        /// 检查任意失败
        /// </summary>
        /// <returns></returns>
        private Status CheckAnyFailure()
        {
            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChild(i);
                var childStatus = child?.Execute() ?? Status.Inactive;
                if (childStatus == Status.Running)
                    return Status.Running;
                if (childStatus == Status.Failure)
                    return Status.Success;
            }
            return Status.Failure;
        }

    }
}
