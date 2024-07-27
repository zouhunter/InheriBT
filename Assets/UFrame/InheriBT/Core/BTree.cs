using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using UnityEditor;

using UnityEngine;

namespace UFrame.InheriBT
{
    [System.Serializable]
    public class BTree : ScriptableObject, IVariableProvider
    {
        [SerializeField]
        protected TreeInfo _rootTree;
        public virtual TreeInfo rootTree
        {
            get { return _rootTree; }
            set { _rootTree = value; }
        }
        public bool TreeStarted => _treeStarted;
        private Dictionary<Type, IEnumerable<FieldInfo>> _fieldMap = new Dictionary<Type, IEnumerable<FieldInfo>>();
        private bool _treeStarted;
        public BTree Owner { get; internal set; }

        #region Variables
        private Dictionary<string, Variable> _variables = new Dictionary<string, Variable>();
        private HashSet<string> _persistentVariables = new HashSet<string>();
        private HashSet<string> _persistentEvents = new HashSet<string>();
        public Variable GetVariable(string name)
        {
            _variables.TryGetValue(name, out var variable);
            return variable;
        }

        public Variable<T> GetVariable<T>(string name)
        {
            return GetVariable<T>(name, false);
        }

        public Variable<T> GetVariable<T>(string name, bool createIfNotExits)
        {
            if (_variables.TryGetValue(name, out var variable))
            {
                if (variable is Variable<T> genVariable)
                    return genVariable;
                else
                {
                    Debug.LogError("variable type miss match:" + name + "," + typeof(T));
                }
            }
            else if (createIfNotExits)
            {
                var newVariable = new Variable<T>();
                _variables[name] = newVariable;
                if (newVariable.Value == null && (typeof(T).IsArray || typeof(T).IsGenericType))
                {
                    newVariable.SetValue(Activator.CreateInstance<T>());
                }
                return newVariable;
            }
            return null;
        }
        public T GetVariableValue<T>(string name)
        {
            if (_variables.TryGetValue(name, out var variable) && variable.GetValue() is T value)
            {
                return value;
            }
            return default(T);
        }
        public bool TryGetVariable<T>(string name, out Variable<T> variable)
        {
            if (_variables.TryGetValue(name, out var variableObj) && variableObj is Variable<T> genVariable && genVariable != null)
            {
                variable = genVariable;
                return true;
            }
            variable = null;
            return false;
        }
        public bool TryGetVariable(string name, out Variable variable)
        {
            return _variables.TryGetValue(name, out variable);
        }

        public void SetVariable(string name, Variable variable)
        {
            _variables[name] = variable;
        }
        public bool SetVariableValue(string name, object data)
        {
            if (_variables.TryGetValue(name, out var variable))
            {
                variable.SetValue(data);
                return true;
            }
            return false;
        }
        public void SetVariableValue<T>(string name, T data)
        {
            var variable = GetVariable<T>(name, true);
            variable.Value = data;
        }
        #endregion Variables

        #region Events
        private Dictionary<string, List<Action<object>>> _events = new Dictionary<string, List<Action<object>>>();
        public void BindingEventMap(Dictionary<string, List<Action<object>>> map)
        {
            this._events = map;
        }
        public void RegistEvent(string eventKey, Action<object> callback)
        {
            if (!_events.TryGetValue(eventKey, out var actions))
            {
                _events[eventKey] = new List<Action<object>>() { callback };
            }
            else
            {
                actions.Add(callback);
            }
        }
        public void RemoveEvent(string eventKey, Action<object> callback)
        {
            if (_events.TryGetValue(eventKey, out var actions))
            {
                actions.Remove(callback);
            }
        }
        public void SendEvent(string eventKey, object arg = null)
        {
            if (_events.TryGetValue(eventKey, out var actions))
            {
                for (int i = actions.Count - 1; i >= 0; i--)
                {
                    try
                    {
                        actions[i]?.Invoke(arg);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }
            }
        }
        #endregion

        #region TreeTraversal
        public BTree CreateInstance()
        {
            return Instantiate(this);
        }

        public void CollectToDic(TreeInfo info, Dictionary<string, TreeInfo> infoDic)
        {
            if (info == null || info.id == null)
                return;

            infoDic[info.id] = info;
            info.subTrees?.ForEach(t =>
            {
                CollectToDic(t, infoDic);
            });
        }

        public virtual TreeInfo FindTreeInfo(string id, bool deep = true)
        {
            var dic = new Dictionary<string, TreeInfo>();
            CollectToDic(rootTree, dic);
            if (dic.TryGetValue(id, out TreeInfo treeInfo))
            {
                return treeInfo;
            }
            return null;
        }

        /// <summary>
        /// 清理上下文
        /// </summary>
        /// <param name="includePersistent"></param>
        public void ClearCondition(bool includePersistent = true)
        {
            if (includePersistent)
            {
                _variables.Clear();
                _events.Clear();
            }
            else
            {
                var keys = new List<string>(_variables.Keys);
                foreach (var key in keys)
                {
                    if (!_persistentVariables.Contains(key))
                    {
                        _variables.Remove(key);
                    }
                }
                keys = new List<string>(_events.Keys);
                foreach (var key in keys)
                {
                    if (!_persistentEvents.Contains(key))
                    {
                        _events.Remove(key);
                    }
                }
            }
        }

        public virtual bool StartUp()
        {
            if (rootTree != null && rootTree.node != null)
            {
                SetOwnerDeepth(rootTree, this);
                _treeStarted = true;
                return true;
            }
            Debug.LogError("rootTree empty!");
            return false;
        }

        public void Stop()
        {
            if (rootTree != null && rootTree.node != null)
            {
                rootTree.node?.Dispose();
            }
            _treeStarted = false;
        }

        void OnDestroy()
        {
            Stop();
        }

        internal bool ResetStart()
        {
            if (rootTree != null && _treeStarted)
            {
                SetOwnerDeepth(rootTree,this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetOwnerDeepth(TreeInfo info,BTree owner)
        {
            info.node?.SetOwner(owner);
            info.status = Status.Inactive;
            if (info.condition.enable && info.condition.conditions != null && info.node == this)
            {
                foreach (var condition in info.condition.conditions)
                {
                    condition.status = Status.Inactive;
                    condition.node?.SetOwner(owner);
                    condition.subConditions?.ForEach(subNode =>
                    {
                        subNode.status = Status.Inactive;
                        subNode?.node?.SetOwner(owner);
                    });
                }
            }
            if (info.subTrees != null && info.subTrees != null)
            {
                foreach (var subInfo in info.subTrees)
                {
                    if (subInfo.enable)
                    {
                        SetOwnerDeepth(subInfo, owner);
                    }
                }
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public virtual Status Tick()
        {
            if (rootTree == null || rootTree.node == null || rootTree.enable == false)
            {
                if (rootTree == null)
                    Debug.LogError("BTree rootTree == null");
                if (rootTree.node == null)
                    Debug.LogError("BTree rootTree.node == null");
                if (!rootTree.enable)
                    Debug.LogError("BTree rootTree.enable == false");
                return Status.Inactive;
            }
            rootTree.status = OnUpdate();
            return rootTree.status;
        }

        internal void OnReset()
        {
            if (rootTree != null && rootTree.node != null)
            {
                SetOwnerDeepth(rootTree, this);
            }
        }

        internal Status OnUpdate()
        {
            return rootTree.node.Execute(rootTree);
        }

        /// <summary>
        /// 嵌套节点检查
        /// </summary>
        /// <param name="matchType"></param>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public virtual bool CheckConditions(TreeInfo treeInfo,MatchType matchType, List<SubConditionItem> conditions)
        {
            if (conditions != null && conditions.Count > 0)
            {
                int matchCount = 0;
                int validCount = 0;

                foreach (var conditionNode in conditions)
                {
                    if (conditionNode != null && conditionNode.node && conditionNode.state < 2)
                        validCount++;
                    else
                        continue;

                    conditionNode.status = conditionNode.node.Execute(treeInfo);
                    var result = conditionNode.status == (conditionNode.state == 1 ? Status.Success:Status.Failure);
#if UNITY_EDITOR
                    Debug.Log("check sub condition:" + conditionNode.node.name + " ," + result);
#endif
                    switch (matchType)
                    {
                        case MatchType.AnySuccess:
                            if (result)
                                return true;
                            break;
                        case MatchType.AnyFailure:
                            if (!result)
                                return true;
                            break;
                        case MatchType.AllSuccess:
                            if (result)
                                matchCount++;
                            else
                                return false;
                            break;
                        case MatchType.AllFailure:
                            if (!result)
                                matchCount++;
                            else
                                return false;
                            break;
                        default:
                            matchCount = -1;
                            break;
                    }
                }
                if (matchCount >= 0 && matchCount != validCount)
                    return false;
            }
            return true;
        }

        /// <summary>
        /// 条件检查
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public virtual bool CheckConditions(TreeInfo treeInfo)
        {
            var conditionInfo = treeInfo.condition;
            if (conditionInfo.enable && conditionInfo.conditions.Count > 0)
            {
                int matchCount = 0;
                int validCount = 0;

                foreach (var condition in conditionInfo.conditions)
                {
                    if (condition != null && condition.node != null && condition.state < 2)
                        validCount++;
                    else
                        continue;

                    bool checkResult = true;
                    if (condition.subEnable)
                        checkResult = CheckConditions(treeInfo,condition.matchType, condition.subConditions);

                    condition.status = condition.node.Execute(treeInfo);
                    if (checkResult)
                        checkResult = condition.status == (condition.state == 1 ? Status.Failure : Status.Success);

                    Debug.Log("checking condition:" + condition.node.name + "," + checkResult + "," + conditionInfo.matchType);
                    switch (conditionInfo.matchType)
                    {
                        case MatchType.AllSuccess:
                            if (checkResult)
                                matchCount++;
                            else
                                return false;
                            break;
                        case MatchType.AllFailure:
                            if (!checkResult)
                                matchCount++;
                            else
                                return false;
                            break;
                        case MatchType.AnySuccess:
                            if (checkResult)
                                return true;
                            matchCount = -1;
                            break;
                        case MatchType.AnyFailure:
                            if (!checkResult)
                                return true;
                            matchCount = -1;
                            break;
                    }
                }
                return matchCount >= 0 && matchCount == validCount;
            }
            return true;
        }
        #endregion


        /// <summary>
        /// 反射获取所有的引用变量
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IEnumerable<FieldInfo> GetTypeRefs(Type type)
        {
            if (!_fieldMap.TryGetValue(type, out var fields))
            {
                fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy).Where(f => typeof(IRef).IsAssignableFrom(f.FieldType));
                _fieldMap[type] = fields;
            }
            return fields;
        }
        /// <summary>
        /// 持久变量
        /// </summary>
        /// <param name="variableName"></param>
        public void SetPersistentVariable(string variableName)
        {
            _persistentVariables.Add(variableName);
        }
        /// <summary>
        /// 持久事件
        /// </summary>
        /// <param name="eventName"></param>
        public void SetPersistentEvent(string eventName)
        {
            _persistentEvents.Add(eventName);
        }

        /// <summary>
        /// 收集节点
        /// </summary>
        /// <param name="allNodes"></param>
        /// <exception cref="NotImplementedException"></exception>
        public virtual void CollectNodesDeepth(TreeInfo info, List<BaseNode> nodes)
        {
            if (info.node && !nodes.Contains(info.node))
            {
                nodes.Add(info.node);
            }
            if (info.condition != null && info.condition.conditions != null)
            {
                int i = 0;
                foreach (var condition in info.condition.conditions)
                {
                    if (condition.node && !nodes.Contains(condition.node))
                    {
                        nodes.Add(condition.node);
                    }

                    if (condition.subConditions != null)
                    {
                        int j = 0;
                        foreach (var subNode in condition.subConditions)
                        {
                            if (subNode != null && subNode.node && !nodes.Contains(subNode.node))
                            {
                                nodes.Add(subNode.node);
                            }
                            j++;
                        }
                    }
                    i++;
                }
            }
            if (info.subTrees != null)
            {
                for (int i = 0; i < info.subTrees.Count; i++)
                {
                    var item = info.subTrees[i];
                    CollectNodesDeepth(item, nodes);
                }
            }
        }
    }
}
