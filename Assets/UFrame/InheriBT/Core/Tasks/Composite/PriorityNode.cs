/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-18
 * Version: 1.0.0
 * Description: 优先执行节点
 *_*/

using System;
using UnityEngine;


namespace UFrame.InheriBT.Composite
{
    public class PriorityNode : SequenceNode
    {
        private int[] _priorityIndexs;
        public override BaseNode GetChild(int index)
        {
            var realIndex = _priorityIndexs[index];
            return base.GetChild(realIndex);
        }

        protected override Status OnUpdate()
        {
            RefreshPriority();
            return base.OnUpdate();
        }

        public void RefreshPriority()
        {
            if (_priorityIndexs == null)
            {
                _priorityIndexs = new int[ChildCount];
                for (int i = 0; i < _priorityIndexs.Length; i++)
                {
                    _priorityIndexs[i] = i;
                }
            }

            for (int i = 0; i < ChildCount; i++)
            {
                var priority = GetChild(i).Priority;
                for (int j = 0; j < _priorityIndexs.Length; j++)
                {
                    if(i == _priorityIndexs[j])
                        break;

                    var lastPriority = GetChild(_priorityIndexs[j]).Priority;   
                    if(priority > lastPriority)
                    {
                        var indexI = Array.IndexOf(_priorityIndexs, i);
                        if(indexI > j)
                        {
                            _priorityIndexs[indexI] = _priorityIndexs[j];
                            _priorityIndexs[j] = i;
                        }
                    }
                }
            }
        }
    }
}
