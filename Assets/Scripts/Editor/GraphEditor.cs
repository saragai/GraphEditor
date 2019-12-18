using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;
using System.Collections.Generic;


public class GraphEditor : EditorWindow
{
    [MenuItem("Window/GraphEditor")]
    public static void ShowWindow()
    {
        GraphEditor graphEditor = CreateInstance<GraphEditor>();
        graphEditor.Show();
        graphEditor.titleContent = new GUIContent("Graph Editor");

        if(Selection.activeObject is GraphAsset graphAsset)
        {
            graphEditor.Initialize(graphAsset);
        }
    }

    [OnOpenAsset()]
    private static bool OnOpenAsset(int instanceId, int line)
    {
        if(EditorUtility.InstanceIDToObject(instanceId) is GraphAsset)
        {
            ShowWindow();
            return true;
        }

        return false;
    }

    GraphAsset m_GraphAsset;
    GraphEditorElement m_GraphEditorElement;

    public void OnEnable()
    {
        if (m_GraphAsset != null)
        {
            Initialize(m_GraphAsset);
        }
    }

    public void Initialize(GraphAsset graphAsset)
    {
        m_GraphAsset = graphAsset;

        VisualElement root = this.rootVisualElement;

        m_GraphEditorElement = new GraphEditorElement(graphAsset);
        root.Add(m_GraphEditorElement);
    }
}

public class GraphEditorElement: VisualElement
{
    GraphAsset m_GraphAsset;

    List<NodeElement> m_Nodes;

    public GraphEditorElement(GraphAsset graphAsset)
    {
        m_GraphAsset = graphAsset;

        style.flexGrow = 1;
        style.overflow = Overflow.Hidden;

        this.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));

        m_Nodes = new List<NodeElement>();

        foreach(var node in graphAsset.nodes)
        {
            CreateNodeElement(node);
        }
    }

    void CreateNodeElement(SerializableNode node)
    {
        var nodeElement = new NodeElement(node);

        Add(nodeElement);
        m_Nodes.Add(nodeElement);
    }

    void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
    {
        if(evt.target != this)
        {
            return;
        }

        evt.menu.AppendAction(
            "Add Node",
            AddNodeMenuAction,
            DropdownMenuAction.AlwaysEnabled
            );
    }

    private void AddNodeMenuAction(DropdownMenuAction menuAction)
    {
        Vector2 mousePosition = menuAction.eventInfo.localMousePosition;
        var node = new SerializableNode() { position = mousePosition };

        m_GraphAsset.nodes.Add(node);

        CreateNodeElement(node);
    }
}
