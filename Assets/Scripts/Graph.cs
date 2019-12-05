using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

[Serializable]
public class Graph
{
    public List<Node> nodes;
}

[Serializable]
public class Node
{
    private List<Edge> m_OutEdges;
    public IEnumerable<Edge> OutEdges { get { return m_OutEdges; } }

    private List<Edge> m_InEdges;
    public IEnumerable<Edge> InEdges { get { return m_InEdges; } }

    public Node()
    {
        m_OutEdges = new List<Edge>();
        m_InEdges = new List<Edge>();
    }

    public void ConnectTo(Node node)
    {
        if (CanConnectTo(node) == false)
        {
            return;
        }

        m_OutEdges.Add(new Edge(node));
        node.m_InEdges.Add(new Edge(this));
    }

    protected virtual bool CanConnectTo(Node node)
    {
        if(m_OutEdges.Exists(e => e.node == node))
        {
            return false;
        }

        return true;
    }
}

[Serializable]
public class Edge
{
    public Node node;

    public Edge(Node node)
    {
        this.node = node;
    }
}
