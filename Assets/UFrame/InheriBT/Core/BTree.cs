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
    public class BTree : ScriptableObject, IBaseNode, IVariableProvider
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

        public Status Status { get; private set; }

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
                rootTree.node.SetOwner(this, rootTree);
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
                rootTree.node.SetOwner(this, rootTree);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 刷新
        /// </summary>
        public virtual Status Tick()
        {
            Status = Status.Inactive;
            if (rootTree == null || rootTree.node == null || rootTree.enable == false)
            {
                if (rootTree == null)
                    Debug.LogError("BTree rootTree == null");
                if (rootTree.node == null)
                    Debug.LogError("BTree rootTree.node == null");
                if (!rootTree.enable)
                    Debug.LogError("BTree rootTree.enable == false");
                return Status;
            }
            Status = OnUpdate();
            return Status;
        }

        internal void OnReset()
        {
            if (rootTree != null && rootTree.node != null)
            {
                rootTree.node.SetOwner(Owner, rootTree);
                rootTree.node.ClearStatus();
            }
        }

        internal Status OnUpdate()
        {
            return rootTree.node.Execute();
        }

        public virtual bool CheckConditions(MatchType matchType, List<SubConditionItem> conditions)
        {
            if (conditions != null && conditions.Count > 0)
            {
                int matchCount = 0;
                int validCount = 0;

                foreach (var conditionNode in conditions)
                {
                    if (conditionNode != null && conditionNode.node)
                        validCount++;
                    else
                        continue;

                    var result = conditionNode?.node?.Execute();
#if UNITY_EDITOR
                    Debug.Log("check sub condition:" + conditionNode.node.name + " ," + result);
#endif
                    switch (matchType)
                    {
                        case MatchType.AnySuccess:
                            if (result == Status.Success)
                                return true;
                            break;
                        case MatchType.AnyFailure:
                            if (result == Status.Failure)
                                return true;
                            break;
                        case MatchType.AllSuccess:
                            if (result == Status.Success)
                                matchCount++;
                            else if (result == Status.Failure)
                                return false;
                            break;
                        case MatchType.AllFailure:
                            if (result == Status.Failure)
                                matchCount++;
                            else if (result == Status.Success)
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
        public virtual bool CheckConditions(ConditionInfo conditionInfo)
        {
            if (conditionInfo.enable && conditionInfo.conditions.Count > 0)
            {
                int matchCount = 0;
                int validCount = 0;

                foreach (var subConditionInfo in conditionInfo.conditions)
                {
                    if (subConditionInfo != null && subConditionInfo.node != null)
                        validCount++;
                    else
                        continue;

                    var checkResult = CheckConditions(subConditionInfo.matchType, subConditionInfo.subConditions);
                    if (checkResult)
                    {
                        checkResult = subConditionInfo.node.Execute() == Status.Success;
#if UNITY_EDITOR
                        Debug.Log("checking condition:" + subConditionInfo.node.name + "," + checkResult + "," + conditionInfo.matchType);
#endif
                    }
                    switch (conditionInfo.matchType)
                    {
                        case MatchType.AllSuccess:
                            if (checkResult)
                                matchCount++;
                            else
                            {
                                return false;
                            }
                            break;
                        case MatchType.AllFailure:
                            if (!checkResult)
                                matchCount++;
                            else
                            {
                                return false;
                            }
                            break;
                        case MatchType.AnySuccess:
                            if (checkResult)
                            {
                                return true;
                            }
                            matchCount = -1;
                            break;
                        case MatchType.AnyFailure:
                            if (!checkResult)
                            {
                                return true;
                            }
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
