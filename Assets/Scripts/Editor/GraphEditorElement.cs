using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
public class GraphEditorElement: VisualElement
{
    GraphAsset m_GraphAsset;

    List<NodeElement> m_Nodes;
    List<EdgeElement> m_Edges;

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

        m_Edges = new List<EdgeElement>();

        foreach(var node in m_Nodes)
        {
            foreach(var edge in node.serializableNode.edges)
            {
                CreateEdgeElement(edge, node, m_Nodes);
            }
        }
    }

    public NodeElement CreateNodeElement(SerializableNode node)
    {
        var nodeElement = new NodeElement(node);

        Add(nodeElement);
        m_Nodes.Add(nodeElement);

        return nodeElement;
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

    public void DrawEdge()
    {
        foreach(var edge in m_Edges)
        {
            edge.DrawEdge();
        }
    }

    public EdgeElement CreateEdgeElement(SerializableEdge edge, NodeElement fromNode, List<NodeElement> nodeElements)
    {
        var edgeElement = new EdgeElement(edge, fromNode, nodeElements[edge.toId]);
        Add(edgeElement);
        m_Edges.Add(edgeElement);

        return edgeElement;
    }

    public EdgeElement CreateEdgeElement(NodeElement fromNode, Vector2 toPosition)
    {
        var edgeElement = new EdgeElement(fromNode, toPosition);
        Add(edgeElement);
        m_Edges.Add(edgeElement);

        return edgeElement;
    }

    public void RemoveEdgeElement(EdgeElement edge)
    {
        if(edge.serializableEdge != null)
        {
            edge.From.serializableNode.edges.Remove(edge.serializableEdge);
        }

        Remove(edge);
        m_Edges.Remove(edge);
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

    public NodeElement GetDesignatedNode(Vector2 position)
    {
        foreach(NodeElement node in m_Nodes)
        {
            if (node.ContainsPoint(node.WorldToLocal(position)))
                return node;
        }

        return null;
    }

    public bool ContainsEdge(NodeElement from, NodeElement to)
    {
        return m_Edges.Exists(edge =>
        {
            return edge.From == from && edge.To == to;
        });
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
}
