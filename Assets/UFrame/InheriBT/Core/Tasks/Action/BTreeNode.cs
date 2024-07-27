/*-*-* Copyright (c) uframe@zht
 * Author: zouhunter
 * Creation Date: 2024-03-29
 * Version: 1.0.0
 * Description: Returns a Status of appoint.
 *_*/

using UFrame.InheriBT;
using UFrame;
using UnityEngine;

namespace UFrame.InheriBT.Actions
{
    [NodePath("BTreeNode")]
    public class BTreeNode : ActionNode
    {
        public BTree tree;
        [SerializeField]
        private BTree _instanceTree;

        public override void SetOwner(BTree owner)
        {
            if (tree)
                _instanceTree = UnityEngine.Object.Instantiate(tree);
            if (_instanceTree)
                _instanceTree.SetOwnerDeepth(_instanceTree.rootTree, owner);
            base.SetOwner(owner);
        }

        protected override void OnReset()
        {
            base.OnReset();
            _instanceTree?.OnReset();
        }

        protected override Status OnUpdate()
        {
            return _instanceTree?.Tick() ?? Status.Failure;
        }
    }
}
