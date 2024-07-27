using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;

using UnityEngine;

namespace UFrame.InheriBT
{
    public abstract class BaseNode : IDisposable
    {
        private BTree _owner;
        [HideInInspector]
        public string name;
        public BTree Owner => _owner;
        public virtual int Priority => 0;

        private bool _started;

        protected bool _conditionFaliure;

        public static implicit operator bool(BaseNode instance) => instance != null;

        public virtual void SetOwner(BTree owner)
        {
            _owner = owner;
            _started = false;
            BindingRefVars(GetRefVars());
            OnReset();
        }

        internal virtual void ClearStatus()
        {
            //TreeInfo.status = Status.Inactive;
            //if (this._treeInfo == null || _treeInfo.node != this)
            //    return;
            //if (_treeInfo.condition.enable && _treeInfo.condition.conditions != null)
            //{
            //    foreach (var condition in _treeInfo.condition.conditions)
            //    {
            //        condition.node?.ClearStatus();
            //        condition.subConditions?.ForEach(subNode => subNode?.node?.ClearStatus());
            //    }
            //}
            //if (_treeInfo.subTrees != null && _treeInfo.subTrees != null)
            //{
            //    foreach (var subInfo in _treeInfo.subTrees)
            //    {
            //        if (subInfo.enable)
            //        {
            //            subInfo.node?.ClearStatus();
            //        }
            //    }
            //}
        }

        public virtual void Dispose()
        {
            //if (this._treeInfo == null || _treeInfo.node != this)
            //    return;

            //if (_treeInfo.condition.enable && _treeInfo.condition.conditions != null && _treeInfo.node == this)
            //{
            //    foreach (var condition in _treeInfo.condition.conditions)
            //    {
            //        condition.node?.Dispose();
            //        condition.subConditions?.ForEach(subNode => subNode?.node?.Dispose());
            //    }
            //}
            //if (_treeInfo.subTrees != null && _treeInfo.subTrees != null)
            //{
            //    foreach (var subInfo in _treeInfo.subTrees)
            //    {
            //        if (subInfo.enable)
            //        {
            //            subInfo.node?.Dispose();
            //        }
            //    }
            //}
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

        protected virtual Status OnUpdate(TreeInfo info)
        {
            return OnUpdate();
        }

        protected virtual Status OnUpdate()
        {
            return Status.Inactive;
        }

        public virtual Status Execute(TreeInfo info)
        {
            if (info == null)
            {
                info.status = Status.Inactive;
                Debug.LogError(info.node.name + ",TreeInfo == null");
                return info.status;
            }

            _conditionFaliure = false;
            if (info.node == this && info.condition != null && info.condition.enable)
            {
                if (!Owner.CheckConditions(info))
                {
                    info.status = Status.Failure;
#if UNITY_EDITOR
                    Debug.Log("condition failed:" + info.node.name);
#endif
                    _conditionFaliure = true;
                    return info.status;
                }
                else
                {
#if UNITY_EDITOR
                    Debug.Log("condition success:" + info.node.name);
#endif
                }
            }

            if (!_started)
            {
                _started = true;
                OnStart();
            }

            info.status = OnUpdate(info);

            if (info.status != Status.Running)
            {
                _started = false;
                OnEnd();
            }
            return info.status;
        }
    }
}
