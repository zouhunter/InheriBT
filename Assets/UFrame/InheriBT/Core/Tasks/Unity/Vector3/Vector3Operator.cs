/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-28
 * Version: 1.0.0
 * Description: 向量+ - *
 *_*/

using System.Collections.Generic;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("Vector3/Operator")]
    public class Vector3Operator : ActionNode
    {
        public Operation operation;
        public Ref<Vector3> inputA;
        public Ref<Vector3> inputB;
        public Ref<Vector3> result;

        public enum Operation
        {
            Add,
            Subtract,
            Scale
        }

        protected override IEnumerable<IRef> GetRefVars()
        {
            return new IRef[] { inputA, inputB, result };
        }

        protected override Status OnUpdate()
        {
            switch (operation)
            {
                case Operation.Add:
                    result.Value = inputA.Value + inputB.Value;
                    break;
                case Operation.Subtract:
                    result.Value = inputA.Value - inputB.Value;
                    break;
                case Operation.Scale:
                    result.Value = Vector3.Scale(inputA.Value, inputB.Value);
                    break;
            }
            return Status.Success;
        }
    }
}

