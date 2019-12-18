using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using UnityEditor.Callbacks;

public class GraphEditor : EditorWindow
{
	[MenuItem( "Window/GraphEditor" )]
	public static void ShowWindow()
	{
		GraphEditor graphEditor = CreateInstance<GraphEditor>();
		graphEditor.Show();
        graphEditor.titleContent = new GUIContent("Graph Editor");

        if (Selection.activeObject is GraphAsset graphAsset)
        {
            graphEditor.Initialize(graphAsset);
        }
        return;
    }

    [OnOpenAsset()]
    static bool OnOpenAsset(int instanceId, int line)
    {
        if (EditorUtility.InstanceIDToObject(instanceId) is GraphAsset)
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

        m_GraphEditorElement = new GraphEditorElement(graphAsset);
        rootVisualElement.Add(m_GraphEditorElement);
    }

    private void OnGUI()
    {
        if (m_GraphEditorElement == null)
        {
            return;
        }

        m_GraphEditorElement.DrawEdge();
    }
}

public class GraphEditorElement : VisualElement
{
    GraphAsset m_GraphAsset;

    List<NodeElement> m_Nodes;
    List<EdgeElement> m_Edges; 

    public GraphEditorElement(GraphAsset graphAsset)
    {
        m_GraphAsset = graphAsset;

        style.flexGrow = 1;
        style.overflow = Overflow.Hidden;

        this.AddManipulator(new ContextualMenuManipulator(OnContextualMenuPopulate));

        m_Nodes = new List<NodeElement>();
        m_Edges = new List<EdgeElement>();

        foreach (var node in m_GraphAsset.nodes)
        {
            CreateNodeElement(node);
        }

        foreach(var node in m_Nodes)
        {
            foreach(var edge in node.serializableNode.edges)
            {
                CreateEdgeElement(edge, node, m_Nodes);
            }
        }
    }

    private void OnContextualMenuPopulate(ContextualMenuPopulateEvent evt)
    {
        if (evt.target is GraphEditorElement)
        {
            evt.menu.AppendAction(
                "Add Node",
                AddNodeMenuAction,
                DropdownMenuAction.AlwaysEnabled);
            evt.menu.AppendSeparator();
        }
    }

    private void AddNodeMenuAction(DropdownMenuAction menuAction)
    {
        Vector2 mousePosition = menuAction.eventInfo.localMousePosition;
        var node = new SerializableNode() { position = mousePosition };

        m_GraphAsset.nodes.Add(node);

        CreateNodeElement(node);
    }

    public void DrawEdge()
    {
        foreach (var edge in m_Edges)
        {
            edge.DrawEdge();
        }
    }

    public NodeElement CreateNodeElement(SerializableNode node)
    {
        var nodeElement = new NodeElement(node, "test");

        Add(nodeElement);
        m_Nodes.Add(nodeElement);

        return nodeElement;
    }

    public void RemoveNodeElement(NodeElement node)
    {
        m_GraphAsset.nodes.Remove(node.serializableNode);

        int id = m_Nodes.IndexOf(node);

        for (int i = m_Edges.Count - 1; i >= 0; i--)
        {
            var edgeElement = m_Edges[i];
            var edge = edgeElement.serializableEdge;

            if (edgeElement.To == node || edgeElement.From == node)
            {
                RemoveEdgeElement(edgeElement);
                continue;
            }

            if (edge.toId > id)
                edge.toId--;
        }

        Remove(node);
        m_Nodes.Remove(node);
    }

    public EdgeElement CreateEdgeElement(SerializableEdge edge, NodeElement fromNode, List<NodeElement> nodeElements)
    {
        var edgeElement = new EdgeElement(edge, fromNode, nodeElements[edge.toId]);

        Add(edgeElement);
        m_Edges.Add(edgeElement);

        return edgeElement;
    }

    public EdgeElement CreateEdgeElement(NodeElement from, Vector2 position)
    {
        var edgeElement = new EdgeElement(from, position);

        Add(edgeElement);
        m_Edges.Add(edgeElement);

        return edgeElement;
    }

    public void RemoveEdgeElement(EdgeElement edge)
    {
        if (edge.serializableEdge != null)
        {
            edge.From.serializableNode.edges.Remove(edge.serializableEdge);
        }

        Remove(edge);
        m_Edges.Remove(edge);
    }

    public void SerializeEdge(EdgeElement edge)
    {
        var serializableEdge = new SerializableEdge()
        {
            toId = m_Nodes.IndexOf(edge.To)
        };

        edge.From.serializableNode.edges.Add(serializableEdge);
        edge.serializableEdge = serializableEdge;
    }

    public bool SameEdgeExists(NodeElement from, NodeElement to)
    {
        return m_Edges.Exists(edge =>
        {
            return edge.From == from && edge.To == to;
        });
    }
}
