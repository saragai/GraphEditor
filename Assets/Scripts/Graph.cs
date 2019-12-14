using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Graph
{
	public List<Node> nodes;

	public Graph( GraphAsset graphAsset )
	{
		nodes = new List<Node>();

		foreach( var serializableNode in graphAsset.nodes )
		{
			Node node = new Node();
			nodes.Add( node );
		}

		for( int i = 0; i < graphAsset.nodes.Count; i++ )
		{
			var serializableNode = graphAsset.nodes[i];

			foreach( var serializableEdge in serializableNode.edges )
			{
				nodes[i].ConnectTo( nodes[serializableEdge.nodeId] );
			}
		}
	}
}

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

	public void ConnectTo( Node node )
	{
		if( CanConnectTo( node ) == false )
		{
			return;
		}

		m_OutEdges.Add( new Edge( node ) );
		node.m_InEdges.Add( new Edge( this ) );
	}

	protected virtual bool CanConnectTo( Node node )
	{
		if( m_OutEdges.Exists( e => e.node == node ) )
		{
			return false;
		}

		return true;
	}
}

public class Edge
{
	public Node node;

	public Edge( Node node )
	{
		this.node = node;
	}
}
