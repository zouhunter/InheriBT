using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace UFrame.InheriBT
{
    public abstract class BaseNode :IDisposable
    {
        private BTree _owner;
        [HideInInspector]
        public string name;
        private TreeInfo _treeInfo;
        public BTree Owner => _owner;
        public TreeInfo TreeInfo => _treeInfo;
        public Status Status { get; protected set; }
        public virtual int Priority => 0;

        private bool _started;

        protected bool _conditionFaliure;

        public static implicit operator bool(BaseNode instance) => instance != null;

        public virtual void SetOwner(BTree owner, TreeInfo info)
        {
            _owner = owner;
            _treeInfo = info;
            Status = Status.Inactive;
            if (info.condition.enable && info.condition.conditions != null && info.node == this)
            {
                foreach (var condition in info.condition.conditions)
                {
                    condition.node?.SetOwner(owner, info);
                    condition.subConditions?.ForEach(subNode => subNode?.node?.SetOwner(owner, info));
                }
            }
            if (info.subTrees != null && info.subTrees != null)
            {
                foreach (var subInfo in info.subTrees)
                {
                    if (subInfo.enable)
                    {
                        subInfo.node?.SetOwner(owner, subInfo);
                    }
                }
            }
            _started = false;
            BindingRefVars(GetRefVars());
            OnReset();
        }

        internal virtual void ClearStatus()
        {
            Status = Status.Inactive;
            if (_treeInfo == null)
                return;
            var info = _treeInfo;
            if (info.condition.enable && info.condition.conditions != null && info.node == this)
            {
                foreach (var condition in info.condition.conditions)
                {
                    condition.node?.ClearStatus();
                    condition.subConditions?.ForEach(subNode => subNode?.node?.ClearStatus());
                }
            }
            if (info.subTrees != null && info.subTrees != null)
            {
                foreach (var subInfo in info.subTrees)
                {
                    if (subInfo.enable)
                    {
                        subInfo.node?.ClearStatus();
                    }
                }
            }
        }

        public virtual void Dispose()
        {
            if (_treeInfo != null)
            {
                if (_treeInfo.condition.enable && _treeInfo.condition.conditions != null && _treeInfo.node == this)
                {
                    foreach (var condition in _treeInfo.condition.conditions)
                    {
                        condition.node?.Dispose();
                        condition.subConditions?.ForEach(subNode => subNode?.node?.Dispose());
                    }
                }
                if (_treeInfo.subTrees != null && _treeInfo.subTrees != null)
                {
                    foreach (var subInfo in TreeInfo.subTrees)
                    {
                        if (subInfo.enable)
                        {
                            subInfo.node?.Dispose();
                        }
                    }
                }
            }
        }

        protected virtual IEnumerable<IRef> GetRefVars()
        {
            return Owner.GetTypeRefs(GetType())
                .Select(f => f.GetValue(this) as IRef)
                .Where(r => r != null);
        }

        protected void BindingRefVars(IEnumerable<IRef> refVars)
        {
            if (refVars != null)
            {
                foreach (var refVar in refVars)
                {
                    refVar?.Binding(Owner);
                }
            }
        }

        protected virtual void OnStart()
        {
            Debug.Assert(Owner != null, "owner is empty！");
        }

        protected virtual void OnReset()
        {
            Debug.Assert(Owner != null, "owner is empty！");
        }
        protected virtual void OnEnd()
        {
            Debug.Assert(Owner != null, "owner is empty！");
        }

        protected abstract Status OnUpdate();

        public virtual Status Execute()
        {
            if (TreeInfo == null)
            {
                Status = Status.Inactive;
                Debug.LogError(_treeInfo.node.name + ",TreeInfo == null");
                return Status;
            }

            _conditionFaliure = false;
            if (TreeInfo.node == this)
            {
                if(!Owner.CheckConditions(TreeInfo.condition))
                {
                    Status = Status.Failure;
#if UNITY_EDITOR
                    Debug.Log("conditon failed:" + _treeInfo.node.name);
#endif
                    _conditionFaliure = true;
                    return Status;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log("conditon success:" + _treeInfo.node.name);
#endif
                }
            }

            if (!_started)
            {
                _started = true;
                OnStart();
            }

            Status = OnUpdate();

            if (Status != Status.Running)
            {
                _started = false;
                OnEnd();
            }
            return Status;
        }
    }

    public interface IBaseNode
    {
        Status Status { get; }
    }
}
