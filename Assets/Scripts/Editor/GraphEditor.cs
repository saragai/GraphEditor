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

        m_GraphEditorElement = new GraphEditorElement(graphAsset, position.height, position.width);
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

    public List<NodeElement> Nodes { get; private set; }
    public List<EdgeElement> Edges { get; private set; }

    public GraphEditorElement(GraphAsset graphAsset, float height, float width)
    {
        m_GraphAsset = graphAsset;

        style.height = height;
        style.width = width;
        style.overflow = Overflow.Hidden;
        style.flexDirection = FlexDirection.Column;
        // pickingMode = PickingMode.Ignore,
        transform.position = new Vector3(0, 0);

        this.AddManipulator(new ContextualMenuManipulator(evt =>
        {
            if (evt.target is GraphEditorElement)
            {
                evt.menu.AppendAction(
                    "Add Node",
                    menuItem =>
                    {
                        AddNode(menuItem.eventInfo.localMousePosition);
                    },
                    DropdownMenuAction.AlwaysEnabled);
            }
        }));

        Nodes = new List<NodeElement>();
        Edges = new List<EdgeElement>();

        foreach (var node in m_GraphAsset.nodes)
        {
            CreateNodeElement(node);
        }

        foreach(var node in Nodes)
        {
            foreach(var edge in node.SerializableNode.edges)
            {
                CreateEdgeElement(edge);
            }
        }
    }

    public void DrawEdge()
    {
        foreach(var edge in Edges)
        {
            edge.DrawEdge();
        }
    }

    public void AddNode(Vector2 position)
    {
        var node = new SerializableNode()
        {
            position = position,
        };

        m_GraphAsset.nodes.Add(node);

        CreateNodeElement(node);
    }

    public NodeElement CreateNodeElement(SerializableNode node)
    {
        var nodeElement = new NodeElement(node, "test");

        Add(nodeElement);
        Nodes.Add(nodeElement);

        return nodeElement;
    }

    public EdgeElement CreateEdgeElement(SerializableEdge edge)
    {
        var edgeElement = new EdgeElement(Nodes[edge.fromId], Nodes[edge.toId]);

        Add(edgeElement);
        Edges.Add(edgeElement);

        return edgeElement;
    }

    public EdgeElement CreateEdgeElement(NodeElement from, Vector2 position)
    {
        var edgeElement = new EdgeElement(from, position);

        Add(edgeElement);
        Edges.Add(edgeElement);

        return edgeElement;
    }

    public void RemoveEdgeElement(EdgeElement edge)
    {
        Remove(edge);
        Edges.Remove(edge);
    }

    public void SerializeEdge(EdgeElement edge)
    {
        var serializableEdge = new SerializableEdge()
        {
            fromId = Nodes.IndexOf(edge.From),
            toId = Nodes.IndexOf(edge.To)
        };

        edge.From.SerializableNode.edges.Add(serializableEdge);
    }
}

public interface INode
{
    Vector2 GetStartPosition();
    Vector2 GetEndPosition();
    Vector2 GetStartNorm();
    Vector2 GetEndNorm();
}
