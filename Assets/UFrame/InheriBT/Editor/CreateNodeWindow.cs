using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;
using System;
using System.Reflection;
using System.Linq;

namespace UFrame.InheriBT
{
    public class CreateNodeWindow : ScriptableObject, ISearchWindowProvider
    {
        private Texture2D icon;
        private Action<BaseNode> createNodeAction;
        private Type baseType;
        private BTree bTree;

        public class ScriptTemplate
        {
            public string templateFile;
            public string subFolder;
            public string defaultFileName;
        }

        public void Initialise(Action<BaseNode> onCreate, Type baseType, BTree tree)
        {
            this.createNodeAction = onCreate;
            this.baseType = baseType;
            this.bTree = tree;
            icon = new Texture2D(1, 1);
            icon.SetPixel(0, 0, new Color(0, 0, 0, 0));
            icon.Apply();
        }

        public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
        {
            var tree = new List<SearchTreeEntry>
            {
                new SearchTreeGroupEntry(new GUIContent("Create Node"), 0),
            };

            CreateTargetTypes("Conditions", typeof(ConditionNode), tree, context);
            CreateTargetTypes("Actions", typeof(ActionNode), tree, context);
            CreateTargetTypes("Composites", typeof(CompositeNode), tree, context);
            CreateTargetTypes("Decorators", typeof(DecorateNode), tree, context);
            CreateFromTreeNodes(tree,context);
            CreateNewScriptSearch(tree, context);
            return tree;
        }

        private void CreateFromTreeNodes(List<SearchTreeEntry> tree, SearchWindowContext context)
        {
            if(bTree != null)
            {
                var allNodes = new List<BaseNode>();
                bTree.CollectNodesDeepth(bTree.rootTree,allNodes);
                allNodes.RemoveAll(x => !baseType.IsAssignableFrom(x.GetType()));
                var groups = allNodes.GroupBy(x => x.GetType().BaseType.Name).ToList();
                if(groups.Count > 0)
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent("Internals")) { level = 1 });
                    foreach (var group in groups)
                    {
                        tree.Add(new SearchTreeGroupEntry(new GUIContent(group.Key)) { level = 2 });
                        foreach (var node in group)
                        {
                            var copyNode = node;
                            var action = new Action(() => { 
                                createNodeAction?.Invoke(copyNode);
                            });
                            tree.Add(new SearchTreeEntry(new GUIContent(node.name)) { level = 3,userData= action });
                        }
                    }
                }
            }
        }

        private void CreateTargetTypes(string title,Type bType, List<SearchTreeEntry> tree, SearchWindowContext context)
        {
            if (!baseType.IsAssignableFrom(bType))
                return;

            tree.Add(new SearchTreeGroupEntry(new GUIContent(title)) { level = 1 });
            var types = TypeCache.GetTypesDerivedFrom(bType).Where(x=>!x.IsAbstract).ToList();
            types.Sort((x, y) => string.Compare(x.Namespace + GetTypeName(x), y.Namespace + GetTypeName(y)));
            var entrySet = new HashSet<string>();
            foreach (var type in types)
            {
                CreateEntryFromNamespace(2, type, entrySet, tree, context);
            }
        }
        private void CreateEntryFromNamespace(int startLevel,Type type, HashSet<string> entrySet, List<SearchTreeEntry> tree, SearchWindowContext context)
        {
            if (type.IsAbstract) 
                return;

            var fullName = GetTypeName(type);
            var paths = fullName.Split(new char[] { '.', '/' });
            var nowPath = "";
            for (int i = 0; i < paths.Length; i++)
            {
                var pathItem = paths[i];
                nowPath += pathItem;

                if (entrySet.Contains(nowPath))
                {
                    continue;
                }
                entrySet.Add(nowPath);

                if (i == paths.Length - 1)
                {
                    tree.Add(new SearchTreeEntry(new GUIContent(pathItem))
                    {
                        level = i + startLevel,
                        userData = new Action(() =>
                        {
                            CreateNode(type, context);
                        })
                    });
                }
                else
                {
                    tree.Add(new SearchTreeGroupEntry(new GUIContent(pathItem)) { level = i + startLevel });
                }
            }
        }

        private void CreateNewScriptSearch(List<SearchTreeEntry> tree, SearchWindowContext context)
        {
            tree.Add(new SearchTreeGroupEntry(new GUIContent("New Script...")) { level = 1 });

            if (baseType.IsAssignableFrom(typeof(ActionNode)))
            {
                System.Action createActionScript = () => CreateScript(TaskType.Action, context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Action Script")) { level = 2, userData = createActionScript });
            }
            if (baseType.IsAssignableFrom(typeof(ConditionNode)))
            {

                System.Action createConditionScript = () => CreateScript(TaskType.Condition, context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Condition Script")) { level = 2, userData = createConditionScript });
            }
            if (baseType.IsAssignableFrom(typeof(CompositeNode)))
            {
                System.Action createCompositeScript = () => CreateScript(TaskType.Composite, context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Composite Script")) { level = 2, userData = createCompositeScript });
            }

            if (baseType.IsAssignableFrom(typeof(DecorateNode)))
            {
                System.Action createDecoratorScript = () => CreateScript(TaskType.Deractor, context);
                tree.Add(new SearchTreeEntry(new GUIContent($"New Decorator Script")) { level = 2, userData = createDecoratorScript });
            }
        }

        public bool OnSelectEntry(SearchTreeEntry searchTreeEntry, SearchWindowContext context)
        {
            System.Action invoke = (System.Action)searchTreeEntry.userData;
            invoke();
            return true;
        }

        public void CreateNode(System.Type type, SearchWindowContext context)
        {
            var node = System.Activator.CreateInstance(type) as BaseNode;
            node.name = GetTypeName(type);
            createNodeAction?.Invoke(node);
        }

        private string GetTypeName(Type type)
        {
            var attribute = type.GetCustomAttribute<NodePathAttribute>();
            if (attribute != null)
            {
                return attribute.desc;
            }
            return type.Name;
        }

        public void CreateScript(TaskType type, SearchWindowContext context)
        {
            //模板创建
            Debug.LogError(type);
        }

        public static void Show(Vector2 mousePosition, Action<BaseNode> source, Type baseType,BTree treeNow = null)
        {
            Vector2 screenPoint = GUIUtility.GUIToScreenPoint(mousePosition);
            CreateNodeWindow searchWindowProvider = CreateInstance<CreateNodeWindow>();
            searchWindowProvider.Initialise(source, baseType, treeNow);
            SearchWindowContext windowContext = new SearchWindowContext(screenPoint, 240, 320);
            SearchWindow.Open(windowContext, searchWindowProvider);
        }
    }
}
