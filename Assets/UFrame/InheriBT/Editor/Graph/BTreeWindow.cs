/***********************************************************************************//*
*  作者: 邹杭特                                                                       *
*  时间: 2024-07-22                                                                   *
*  版本:                                                                *
*  功能:                                                                              *
*   - editor                                                                          *
*//************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.InteropServices;

using UnityEditor;
using UnityEditor.Callbacks;

using UnityEngine;

namespace UFrame.InheriBT
{
    public class BTreeWindow : EditorWindow
    {
        [OnOpenAsset(OnOpenAssetAttributeMode.Execute)]
        public static bool OnOpen(int instanceID, int line)
        {
            var obj = EditorUtility.InstanceIDToObject(instanceID);
            if (obj is BTree tree)
            {
                GetWindow<BTreeWindow>().SelectBTree(tree);
                return true;
            }
            return false;
        }

        private BTree bTree;
        private ScrollViewContainer _scrollViewContainer;
        private GraphBackground _background;
        private Rect _graphRegion;
        private List<NodeView> _nodes;
        private List<KeyValuePair<NodeView, NodeView>> _connections;
        private List<BTree> _trees = new List<BTree>();
        private Rect _infoRegion;
        private bool _inConnect;
        private Vector2 _startConnectPos;
        private NodeView _activeNodeView;
        private Dictionary<BaseNode, string> _propPaths;
        private SerializedObject serializedObject;
        private List<BaseNode> _activeNodes;
        private void OnEnable()
        {
            _background = new GraphBackground("UI/Default");
            _graphRegion = position;
            _trees = new List<BTree>();
            _scrollViewContainer = new ScrollViewContainer();
            _scrollViewContainer.Start(rootVisualElement, GetGraphRegion());
            _scrollViewContainer.onGUI = DrawNodeGraphContent;
            RefreshGraphRegion();
            LoadNodeInfos();
            FindCached();
        }

        private void FindCached()
        {
            var treeIds = EditorPrefs.GetString("BTreeWindow.bTrees");
            if (!string.IsNullOrEmpty(treeIds))
            {
                var trees = treeIds.Split("|").Select(x => EditorUtility.InstanceIDToObject(int.Parse(x)) as BTree).ToList();
                trees.RemoveAll(x => !x);
                _trees.AddRange(trees);
            }
        }

        private Rect GetGraphRegion()
        {
            var graphRegion = new Rect(0, EditorGUIUtility.singleLineHeight, position.width * 0.7f, position.height - 2 * EditorGUIUtility.singleLineHeight);
            return graphRegion;
        }

        private void LoadNodeInfos()
        {
            _nodes = new List<NodeView>();
            _connections = new List<KeyValuePair<NodeView, NodeView>>();
            if (bTree != null)
            {
                var rootTree = bTree.rootTree;
                var posMap = new Dictionary<TreeInfo, Vector2Int>();
                CalculateNodePositions(rootTree, 0, 0, posMap);
                MoveOffsetNodePostions(posMap);
                CreateViewDeepth(bTree, rootTree, rootTree, posMap, _nodes);
                var nodes = new List<BaseNode>();
                _propPaths = new Dictionary<BaseNode, string>();
                if (bTree is BVariantTree bvt)
                    bvt.BuildRootTree();
                BTreeDrawer.CollectNodeDeepth(bTree.rootTree, "_rootTree", nodes, _propPaths);
                serializedObject = new SerializedObject(bTree);
            }
           
            Repaint();
        }

        private NodeView CreateViewDeepth(BTree bTree, TreeInfo rootInfo, TreeInfo info, Dictionary<TreeInfo, Vector2Int> posMap, List<NodeView> _nodes)
        {
            var view = new NodeView(bTree, rootInfo, info, posMap[info]);
            view.onReload = LoadNodeInfos;
            view.onStartConnect = OnStartConnect;
            view.onActive = OnSetActiveView;
            this._nodes.Add(view);

            if (info.subTrees != null && info.subTrees.Count > 0)
            {
                for (var i = 0; i < info.subTrees.Count; ++i)
                {
                    var child = info.subTrees[i];
                    var childView = CreateViewDeepth(bTree, rootInfo, child, posMap, _nodes);
                    _connections.Add(new KeyValuePair<NodeView, NodeView>(view, childView));
                }
            }
            return view;
        }

        private void OnSetActiveView(NodeView nodeView)
        {
            _activeNodeView = nodeView;
        }

        private void OnStartConnect(Vector2 pos)
        {
            _startConnectPos = pos;
            _inConnect = true;
        }

        private void MoveOffsetNodePostions(Dictionary<TreeInfo, Vector2Int> posMap)
        {
            var posxArr = posMap.Values.Select(p => p.x).ToArray();
            var max = Mathf.Max(posxArr);
            var min = Mathf.Min(posxArr);
            var offset = Mathf.FloorToInt((max - min) * 0.5f);
            foreach (var node in posMap.Keys.ToArray())
            {
                var pos = posMap[node];
                pos.x -= offset;
                posMap[node] = pos;
            }
        }
        private void CalculateNodePositions(TreeInfo node, int depth, int offset, Dictionary<TreeInfo, Vector2Int> posMap)
        {
            Vector2Int pos = new Vector2Int(0, 0);
            int verticalSpacing = 10; // 垂直间距
            int horizontalSpacing = 10; // 水平间距

            pos.y = depth * (NodeView.MAX_HEIGHT + verticalSpacing);
            int subtreeOffset = offset;

            if (node.subTrees != null && node.subTrees.Count > 0)
            {
                foreach (var child in node.subTrees)
                {
                    CalculateNodePositions(child, depth + 1, subtreeOffset, posMap);
                    subtreeOffset += GetSubtreeWidth(child) + horizontalSpacing;
                }

                int firstChildX = posMap[node.subTrees[0]].x;
                int lastChildX = posMap[node.subTrees[node.subTrees.Count - 1]].x;
                pos.x = (firstChildX + lastChildX) / 2;
            }
            else
            {
                pos.x = offset + NodeView.WIDTH / 2;
            }

            posMap[node] = pos;
        }

        private int GetSubtreeWidth(TreeInfo node)
        {
            int horizontalSpacing = 10; // 水平间距
            int nodeWidth = NodeView.WIDTH; // 节点宽度
            if (node.subTrees == null || node.subTrees.Count == 0)
            {
                return nodeWidth; // 节点宽度
            }

            int width = 0;
            foreach (var child in node.subTrees)
            {
                width += GetSubtreeWidth(child) + horizontalSpacing; // 调整后的水平间距
            }

            return width - horizontalSpacing;
        }

        public void SelectBTree(BTree tree)
        {
            this.bTree = tree;
            LoadNodeInfos();
            EditorPrefs.SetInt("BTreeWindow.bTree", tree.GetInstanceID());
            if (!_trees.Contains(tree))
            {
                _trees.Add(tree);
                SaveTitleBTrees();
            }
        }

        private void SaveTitleBTrees()
        {
            _trees.RemoveAll(x => !x);
            EditorPrefs.SetString("BTreeWindow.bTrees", string.Join("|", _trees.Select(x => x.GetInstanceID()).ToArray()));
        }

        private void OnGUI()
        {
            DrawSelectTree();
            if (!bTree)
            {
                bTree = EditorUtility.InstanceIDToObject(EditorPrefs.GetInt("BTreeWindow.bTree")) as BTree;
                if (bTree)
                    SelectBTree(bTree);
                else
                    return;
            }
            _background?.Draw(_graphRegion, _scrollViewContainer.scrollOffset, _scrollViewContainer.ZoomSize);
            RefreshGraphRegion();
            DrawInformations();
            DrawFootPrint();
        }

        private void DrawSelectTree()
        {
            EditorGUI.LabelField(new Rect(10, 0, 50, EditorGUIUtility.singleLineHeight), "[Trees]:", EditorStyles.boldLabel);
            var rect = new Rect(60, 0, 120, EditorGUIUtility.singleLineHeight);
            for (int i = 0; i < _trees.Count; i++)
            {
                var boxRect = new Rect(rect.x, rect.y, rect.width + 20, rect.height);
                GUI.Box(boxRect, "");
                var active = _trees[i] == bTree;
                var style = active ? EditorStyles.toolbarDropDown : EditorStyles.toolbarButton;
                if (_trees[i] == null)
                    continue;

                if (EditorGUI.ToggleLeft(rect, _trees[i].name, active, style) && !active)
                {
                    SelectBTree(_trees[i]);
                    active = true;
                }
                var btn = new Rect(rect.max.x, 0, 15, 15);
                if (_trees.Count > 1 && GUI.Button(btn, "x"))
                {
                    _trees.RemoveAt(i);
                    if (active)
                    {
                        SelectBTree(_trees[0]);
                    }
                    SaveTitleBTrees();
                    break;
                }
                rect.x += rect.width + 20;
            }
        }

        private void RefreshGraphRegion()
        {
            var graphRegion = GetGraphRegion();
            if (_scrollViewContainer != null && _graphRegion != graphRegion)
            {
                _graphRegion = graphRegion;
                _scrollViewContainer.UpdateScale(graphRegion);
                _scrollViewContainer.Refesh();
            }
            _scrollViewContainer.ApplyOffset();
        }

        private void DrawConnections()
        {
            if (_connections == null)
                return;

            // 绘制连接线
            foreach (var connection in _connections)
            {
                var start = connection.Key;
                var end = connection.Value;
                DrawNodeCurve(start, end);
            }
        }

        void DrawNodeCurve(NodeView startView, NodeView endView)
        {
            if (_scrollViewContainer == null)
                return;
            var active = startView.Info.enable && endView.Info.enable;
            var start = new Vector2(startView.Rect.center.x + 2, startView.Rect.max.y - 2);
            var end = new Vector2(endView.Rect.center.x + 2, endView.Rect.min.y + 2);
            var color = active ? Color.white : Color.gray;
            DrawCurve(start, end, color);
        }

        void DrawCurve(Vector2 start, Vector2 end, Color color)
        {
            Handles.BeginGUI();
            Vector2 startTan = start + Vector2.up * 50; // 控制点向右延伸50像素
            Vector2 endTan = end + Vector2.down * 50; // 控制点向左延伸50像素
            Handles.DrawBezier(start, end, startTan, endTan, color, null, 2);
            Handles.EndGUI();
        }

        private void DrawNodeGraphContent()
        {
            var contentRect = new Rect(Vector2.zero, _graphRegion.size / _scrollViewContainer.minZoomSize);
            BeginWindows();
            _nodes?.ForEach(node =>
            {
                node.DrawNode(contentRect);
            });
            HandleDragNodes();
            EndWindows();
            DrawConnections();
        }

        private void HandleDragNodes()
        {
            if (_inConnect && Event.current.type == EventType.MouseUp)
            {
                _inConnect = false;
                _activeNodeView?.CreateAndAddChild();
            }

            if (_inConnect)
            {
                var start = _startConnectPos;
                var end = Event.current.mousePosition;
                DrawCurve(start, end, Color.cyan);
            }
        }
        private void DrawFootPrint()
        {
            var region = new Rect(0, position.height - EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight);
            if (GUI.Button(region, "zouhangte@wekoi.cn,power by uframe", EditorStyles.linkLabel))
            {
                Application.OpenURL("https://alidocs.dingtalk.com/i/nodes/gvNG4YZ7Jne60YR3f2jYBoPyV2LD0oRE?doc_type=wiki_doc&orderType=SORT_KEY&rnd=0.48325224514433396&sortType=DESC");
            }
        }

        private void DrawTitleInfo(Rect contentRect)
        {
            GUIStyle centeredStyle = new GUIStyle(EditorStyles.boldLabel);
            centeredStyle.alignment = TextAnchor.MiddleLeft;
            centeredStyle.fontSize = 14; // 设置字体大小
            centeredStyle.richText = true;
            var name = bTree != null ? bTree.name : "(none)";
            GUIContent content = new GUIContent($"Unity-动作行为树 <size=12><b><color=black>v1.0</color></b></size> <size=12><b>({name})</b></size>");
            if (GUI.Button(contentRect, content, centeredStyle) && bTree != null)
            {
                EditorGUIUtility.PingObject(bTree);
            }
            var readMeRect = new Rect(contentRect.x + contentRect.width - 60, contentRect.max.y - 20, 60, EditorGUIUtility.singleLineHeight);
            if (EditorGUI.LinkButton(readMeRect, "README"))
                Application.OpenURL("http://");
            var lineRect = new Rect(contentRect.x, contentRect.y + contentRect.height, contentRect.width, 3);
            GUI.Box(lineRect, "");
        }

        private void CollectNodes()
        {
            _activeNodes = _activeNodes ?? new List<BaseNode>();
            _activeNodes.Clear();
            if (_activeNodeView != null)
            {
                if (_activeNodeView.Info.node)
                    _activeNodes.Add(_activeNodeView.Info.node);
                if (_activeNodeView.Info.condition.conditions != null)
                {
                    foreach (var condition in _activeNodeView.Info.condition.conditions)
                    {
                        _activeNodes.Add(condition.node);
                        if(condition.subConditions != null)
                        {
                            foreach (var item in condition.subConditions)
                            {
                                _activeNodes.Add(item.node);
                            }
                        }
                    }
                }

            }
        }
        private void DrawNodeProps(Rect rect)
        {
            serializedObject?.Update();
            int i = 1;
            foreach (var node in _activeNodes)
            {
                if (!node)
                    continue;
                float startY = rect.y;
                if (node)
                {
                    var titleStyle = new GUIStyle(EditorStyles.boldLabel)
                    {
                        fontSize = 12,
                        alignment = TextAnchor.MiddleLeft,
                        normal = { textColor = Color.white * 0.6f }
                    };
                    EditorGUI.DrawTextureTransparent(rect, EditorGUIUtility.IconContent("gameviewbackground@2x").image);
                    GUI.Button(rect, i++.ToString() + "," + _activeNodeView.Info.node.GetType().FullName, titleStyle);
                    rect.y += EditorGUIUtility.singleLineHeight + 2;
                }

                var nodeItemRect = rect;
                nodeItemRect.x += 20;
                nodeItemRect.width -= 20;
                TreeInfoDrawer.DrawCreateNodeContent(nodeItemRect, node, (x) =>
                {
                    _activeNodeView.Info.node = x;
                }, bTree);

                if (node && _propPaths.TryGetValue(node, out var path))
                {
                    var drawer = serializedObject.FindProperty(path);
                    if (drawer != null)
                    {
                        var labelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = 80;
                        rect.height = EditorGUI.GetPropertyHeight(drawer, true);
                        EditorGUI.PropertyField(rect, drawer, GUIContent.none, true);
                        EditorGUIUtility.labelWidth = labelWidth;
                        rect.y += rect.height;
                    }
                }
                var endY = rect.y + rect.height;
                rect.y += 30;
                rect.height = EditorGUIUtility.singleLineHeight;
            }
            serializedObject?.ApplyModifiedProperties();
        }

        private void DrawInformations()
        {
            _infoRegion = new Rect(position.width * 0.7f + 5, EditorGUIUtility.singleLineHeight, position.width * 0.3f - 5, position.height - 2 * EditorGUIUtility.singleLineHeight);
            var contentRect = new Rect(_infoRegion.x, _infoRegion.y, _infoRegion.width, 2 * EditorGUIUtility.singleLineHeight);
            DrawTitleInfo(contentRect);

            if (_activeNodeView == null || _activeNodeView.Info == null)
                return;

            var infoRect = new Rect(_infoRegion.x, contentRect.yMax + 5, _infoRegion.width, _infoRegion.height - EditorGUIUtility.singleLineHeight * 2);
            var rect = new Rect(infoRect.x, infoRect.y, infoRect.width, EditorGUIUtility.singleLineHeight);
            CollectNodes();
            if(_activeNodes.Count > 0)
            {
                DrawNodeProps(rect);
            }


            var descRect = new Rect(_infoRegion.x + 10, _infoRegion.y + _infoRegion.height - 120, _infoRegion.width - 20, 120);
            _activeNodeView.Info.desc = EditorGUI.TextArea(descRect, _activeNodeView.Info.desc);
            if (string.IsNullOrEmpty(_activeNodeView.Info.desc))
            {
                descRect.height = EditorGUIUtility.singleLineHeight;
                EditorGUI.LabelField(descRect, "Description...",EditorStyles.centeredGreyMiniLabel);
            }
        }
    }
}
