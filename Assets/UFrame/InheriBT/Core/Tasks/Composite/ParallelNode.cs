/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-18
 * Version: 1.0.0
 * Description: 
 *_*/

using UnityEngine;

namespace UFrame.InheriBT.Composite
{
    public class ParallelNode : CompositeNode
    {
        [SerializeField]
        private MatchType _abortType;
        public override MatchType abortType => _abortType;

        protected override Status OnUpdate()
        {
            if (ChildCount == 0)
                return Status.Inactive;

            var resultStatus = Status.Failure;
            var successCount = 0;
            var failureCount = 0;
            for (int i = 0; i < ChildCount; i++)
            {
                var child = GetChild(i);
                var childStatus = child.Execute();
                if (childStatus == Status.Inactive)
                    continue;

                switch (childStatus)
                {
                    case Status.Inactive:
                        continue;
                    case Status.Running:
                        return Status.Running;
                    case Status.Failure:
                        if(abortType == MatchType.AnyFailure)
                            resultStatus = Status.Success;
                        else if(abortType == MatchType.AllSuccess)
                            resultStatus = Status.Failure;
                        failureCount++;
                        break;
                    case Status.Success:
                        if(abortType == MatchType.AnySuccess)
                            resultStatus = Status.Success;
                        else if(abortType == MatchType.AllFailure)
                            resultStatus = Status.Failure;
                        successCount++;
                        break;
                    default:
                        break;
                }
            }
            if (abortType == MatchType.AllSuccess && successCount == ChildCount)
                resultStatus = Status.Success;
            else if(abortType == MatchType.AllFailure && failureCount == ChildCount)
                resultStatus = Status.Success;
            return resultStatus;
        }
    }
}
