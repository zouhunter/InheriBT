using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace UFrame.InheriBT
{
    public abstract class ParentNode : BaseNode
    {
        public virtual int maxChildCount { get => int.MaxValue; }

        /// <summary>
        /// 获取有效子树
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int GetChildCount(TreeInfo info)
        {
            if (info.subTrees == null)
                return 0;
            return info.subTrees.Where(x => x.enable).Count();
        }

        /// <summary>
        /// 查找子节点
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public virtual TreeInfo GetChild(TreeInfo info, int index)
        {
            if (info.subTrees == null)
                return null;
            var id = -1;
            foreach (TreeInfo child in info.subTrees)
            {
                if (child.enable)
                    id++;
                if (id == index)
                    return child;
            }
            return null;
        }
    }
}
