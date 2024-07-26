using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace UFrame.InheriBT
{
    public abstract class ParentNode : BaseNode
    {
        public virtual int maxChildCount { get => int.MaxValue; }

        public int ChildCount => _childNodes != null ? _childNodes.Count :0;
        private List<BaseNode> _childNodes;

        /// <summary>
        /// set owner
        /// </summary>
        /// <param name="owner"></param>
        /// <param name="info"></param>
        public override void SetOwner(BTree owner, TreeInfo info)
        {
            base.SetOwner(owner, info);
            if (TreeInfo.subTrees != null && TreeInfo.subTrees != null)
            {
                _childNodes = new List<BaseNode>();
                foreach (var treeInfo in TreeInfo.subTrees)
                {
                    if (treeInfo.enable)
                    {
                        _childNodes.Add(treeInfo.node);
                    }
                }
            }
        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual BaseNode GetChild(int index)
        {
            if (index < 0 || index >= ChildCount)
                return null;
            if(_childNodes != null && _childNodes.Count > index)
            {
                return _childNodes[index];
            }
            return null;
        }
    }
}
