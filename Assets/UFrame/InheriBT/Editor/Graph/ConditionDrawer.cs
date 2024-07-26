/*-*-* Copyright (c) mateai@wekoi
 * Author: zouhunter
 * Creation Date: 2024-07-24
 * Version: 1.0.0
 * Description: 
 *_*/

using UnityEngine;
using System;
using UFrame.InheriBT;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine.UIElements;
using System.Reflection;

namespace UFrame.InheriBT
{
    public class ConditionDrawer
    {

        private ReorderableList _conditionList;
        private TreeInfo _treeInfo;
        private BTree _tree;
        private Dictionary<int, ReorderableList> _subConditionListMap = new Dictionary<int, ReorderableList>();
        public ConditionDrawer(BTree bTree, TreeInfo info)
        {
            _tree = bTree;
            _treeInfo = info;
            bool canAddOrDelete = !(bTree is BVariantTree);
            if (_treeInfo.condition.conditions == null)
                _treeInfo.condition.conditions = new List<ConditionItem>();
            _conditionList = new ReorderableList(_treeInfo.condition.conditions, typeof(ConditionNode), true, true, canAddOrDelete, canAddOrDelete);
            _conditionList.drawHeaderCallback = OnDrawConditionHead;
            _conditionList.onAddCallback = OnAddCondition;
            _conditionList.onRemoveCallback = OnDeleteCondition;
            _conditionList.elementHeightCallback = OnDrawConditonHight;
            _conditionList.drawElementCallback = OnDrawCondtionElement;
        }

        public float GetHeight()
        {
            return _conditionList.GetHeight();
        }

        public void OnGUI(Rect rect)
        {
            _conditionList.DoList(rect);
        }

        protected TreeInfo TreeInfoInBase(TreeInfo info)
        {
            if (_tree is BVariantTree variantTree && variantTree.baseTree)
            {
                return variantTree.baseTree.FindTreeInfo(info.id);
            }
            return null;
        }

        private void OnDrawConditionHead(Rect rect)
        {
            var color = Color.yellow;
            if (_treeInfo.node && _treeInfo.node.Status == Status.Success)
                color = Color.green;

            using (var c = new ColorScope(true, Color.yellow))
                EditorGUI.LabelField(rect, "Conditions");

            var matchRect = new Rect(rect.x + rect.width - 75, rect.y, 75, EditorGUIUtility.singleLineHeight);
            _treeInfo.condition.matchType = (MatchType)EditorGUI.EnumPopup(matchRect, _treeInfo.condition.matchType, EditorStyles.linkLabel);
        }

        private void OnDrawCondtionElement(Rect rect, int index, bool isActive, bool isFocused)
        {
            OnDrawCondition(rect, index);
        }

        private bool CheckConditionChanged(int index, out bool matchTypeGreen, out bool subEnableGreen)
        {
            var baseTreeInfo = TreeInfoInBase(_treeInfo);
            bool green = false;
            matchTypeGreen = false;
            subEnableGreen = false;
            if (_tree is BVariantTree && !EditorApplication.isPlaying && baseTreeInfo?.condition?.conditions != null && _treeInfo?.condition?.conditions != null)
            {
                if (_treeInfo.condition.conditions.Count > index && baseTreeInfo.condition.conditions.Count > index)
                {
                    if ((baseTreeInfo.condition.conditions[index].subEnable != _treeInfo.condition.conditions[index].subEnable))
                    {
                        green = true;
                        subEnableGreen = true;
                    }
                    if (baseTreeInfo.condition.conditions[index].matchType != _treeInfo.condition.conditions[index].matchType)
                    {
                        green = true;
                        matchTypeGreen = true;
                    }
                }
                else if (baseTreeInfo.condition.conditions.Count <= index && index != 0)
                {
                    green = true;
                }
            }
            return green;
        }

        private float OnDrawConditonHight(int index)
        {
            var height = EditorGUIUtility.singleLineHeight;
            if (_treeInfo.condition.conditions.Count > index)
            {
                var condition = _treeInfo.condition.conditions[index];
                if (condition == null)
                    _treeInfo.condition.conditions[index] = condition = new ConditionItem();

                if (condition.subEnable)
                {
                    height += EditorGUIUtility.singleLineHeight * 2.5f;
                    if (condition.subConditions != null && condition.subConditions.Count > 0)
                    {
                        height += EditorGUIUtility.singleLineHeight * condition.subConditions.Count;
                    }
                }
            }
            return height;
        }

        private void OnDrawCondition(Rect rect, int index)
        {
            if (_treeInfo.condition.conditions == null || _treeInfo.condition.conditions.Count <= index)
                return;
            var green = CheckConditionChanged(index, out var matchTypeGreen, out var enableToggleGreen);
            var condition = _treeInfo.condition.conditions[index];
            if (condition == null)
                condition = _treeInfo.condition.conditions[index] = new ConditionItem();
            var objectRect = new Rect(rect.x, rect.y, rect.width - 30, EditorGUIUtility.singleLineHeight);
            var enableRect = new Rect(rect.x + rect.width - 20, rect.y, 20, EditorGUIUtility.singleLineHeight);
            using (var colorScope = new ColorScope(enableToggleGreen, Color.green))
            {
                TreeInfoDrawer.DrawCreateNodeContent(objectRect, condition.node, n =>
                {
                    RecordUndo("condition node changed!");
                    condition.node = n;
                }, _tree);
                DrawNodeState(condition.node, objectRect);

                condition.subEnable = EditorGUI.Toggle(enableRect, condition.subEnable, EditorStyles.radioButton);
            }

            if (condition.subEnable)
            {
                var hashCode = condition.GetHashCode();
                if (!_subConditionListMap.TryGetValue(hashCode, out var subConditionList))
                {
                    if (condition.subConditions == null)
                        condition.subConditions = new List<SubConditionItem>();
                    subConditionList = _subConditionListMap[hashCode] = new ReorderableList(condition.subConditions, typeof(ConditionNode), true, true, true, true);
                    //subConditionList.drawHeaderCallback = (subRect) => { EditorGUI.LabelField(subRect, "SubConditions"); };
                    subConditionList.headerHeight = 0;
                    subConditionList.drawElementCallback = (subRect, subIndex, subIsActive, subIsFocused) =>
                    {
                        var subNode = condition.subConditions[subIndex];
                        if (subNode == null)
                            subNode = new SubConditionItem();
                        var subObjectRect = new Rect(subRect.x, subRect.y, subRect.width, EditorGUIUtility.singleLineHeight);
                        TreeInfoDrawer.DrawCreateNodeContent(subObjectRect, subNode.node, n =>
                        {
                            RecordUndo("condition sub node changed!");
                            subNode.node = n;
                        }, _tree);
                        DrawNodeState(subNode.node, subObjectRect);
                        condition.subConditions[subIndex] = subNode;
                        var subMenuRect = new Rect(subRect.x - 100, subRect.y, 100, EditorGUIUtility.singleLineHeight);
                        if (subMenuRect.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp && Event.current.button == 1)
                        {
                            var menu = new GenericMenu();
                            if (CopyPasteUtil.copyNode && CopyPasteUtil.copyNode is ConditionNode cdn)
                            {
                                menu.AddItem(new GUIContent("Paste"), false, (x) =>
                                {
                                    RecordUndo("paste node");
                                    condition.subConditions[subIndex].node = cdn;
                                }, 0);
                            }
                            if (subNode.node)
                            {
                                menu.AddItem(new GUIContent("Copy"), false, (x) =>
                                {
                                    CopyPasteUtil.copyNode = subNode.node;
                                }, 0);
                            }
                            menu.ShowAsContext();
                        }
                    };
                    subConditionList.onAddCallback = (subList) =>
                    {
                        var baseTreeInfo = TreeInfoInBase(_treeInfo);
                        if (baseTreeInfo != null)
                        {
                            EditorUtility.DisplayDialog("Error", "Can't add base tree sub conditions", "OK");
                            return;
                        }
                        condition.subConditions.Add(null);
                    };
                    subConditionList.onRemoveCallback = (subList) =>
                    {
                        var baseTreeInfo = TreeInfoInBase(_treeInfo);
                        if (baseTreeInfo != null)
                        {
                            EditorUtility.DisplayDialog("Error", "Can't delete base tree sub conditions", "OK");
                            return;
                        }
                        RecordUndo("remove sub condition element");
                        condition.subConditions.RemoveAt(subList.index);
                    };
                }
                using (var colorScope = new ColorScope(green, Color.green))
                {
                    subConditionList.DoList(new Rect(rect.x, rect.y + EditorGUIUtility.singleLineHeight, rect.width, subConditionList.GetHeight()));
                }
            }

            if (condition.subEnable)
            {
                var verticlOffset = EditorGUIUtility.singleLineHeight;
                if (condition.subConditions != null && condition.subConditions.Count > 0)
                {
                    verticlOffset += (EditorGUIUtility.singleLineHeight + 4) * condition.subConditions.Count;
                }

                if (condition.subConditions != null && condition.subConditions.Count > 0)
                {
                    var matchTypeRect = new Rect(rect.x + 20, rect.y + verticlOffset, 75, EditorGUIUtility.singleLineHeight);
                    using(var colorScope = new ColorScope(matchTypeGreen, new Color(0, 1, 0, 0.8f)))
                    {
                        condition.matchType = (MatchType)EditorGUI.EnumPopup(matchTypeRect, condition.matchType, EditorStyles.linkLabel);
                    }
                }
            }

            var subEnableRect1 = new Rect(rect.x - 100, rect.y, 100, EditorGUIUtility.singleLineHeight);
            if (subEnableRect1.Contains(Event.current.mousePosition) && Event.current.type == EventType.MouseUp && Event.current.button == 1)
            {
                var menu = new GenericMenu();
                if (CopyPasteUtil.copyNode && CopyPasteUtil.copyNode is ConditionNode cdn)
                {
                    menu.AddItem(new GUIContent("Paste"), false, (x) =>
                    {
                        RecordUndo("paste node");
                        condition.node = cdn;
                    }, 0);
                }

                if (condition.node)
                {
                    menu.AddItem(new GUIContent("Copy"), false, (x) =>
                    {
                        CopyPasteUtil.copyNode = condition.node;
                    }, 0);
                }
                menu.ShowAsContext();
            }
        }

        private void DrawNodeState(BaseNode node, Rect rect)
        {
            var color = Color.gray;
            var show = false;
            if (node)
            {
                if (node.Status == Status.Success)
                {
                    show = true;
                    color = Color.green;
                }
                else if (node.Status == Status.Failure)
                {
                    show = true;
                    color = Color.red;
                }
                else if (node.Status == Status.Running)
                {
                    show = true;
                    color = Color.yellow;
                }
            }
            if (show)
            {
                using (var colorScope = new ColorGUIScope(true, color))
                {
                    GUI.Box(rect, "");
                }
            }
        }

        private void OnDeleteCondition(ReorderableList list)
        {
            var baseTreeInfo = TreeInfoInBase(_treeInfo);
            if (baseTreeInfo != null)
            {
                EditorUtility.DisplayDialog("Error", "Can't delete base tree sub condition", "OK");
                return;
            }

            RecordUndo("remove condition element");
            _treeInfo.condition.conditions.RemoveAt(list.index);
        }

        private void OnAddCondition(ReorderableList list)
        {
            var baseTreeInfo = TreeInfoInBase(_treeInfo);
            if (baseTreeInfo != null)
            {
                EditorUtility.DisplayDialog("Error", "Can't add base tree sub condition", "OK");
                return;
            }
            RecordUndo("add condition element");
            _treeInfo.condition.conditions.Add(new ConditionItem());
        }

        private void RecordUndo(string info)
        {
            Undo.RecordObject(_tree, info);
        }
    }
}

