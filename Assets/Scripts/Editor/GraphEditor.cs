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
		graphEditor.titleContent = new GUIContent( "Graph Editor" );

		if( Selection.activeObject is GraphAsset graphAsset )
		{
			graphEditor.m_GraphAsset = graphAsset;
		}
	}

	[OnOpenAsset()]
	static bool OnOpenAsset( int instanceId, int line )
	{
		if( EditorUtility.InstanceIDToObject( instanceId ) is GraphAsset )
		{
			ShowWindow();
			return true;
		}

		return false;
	}

	GraphAsset m_GraphAsset;
	List<NodeElement> m_Nodes = new List<NodeElement>();
	Dictionary<Node, NodeElement>

	public void OnEnable()
	{
		VisualElement root = this.rootVisualElement;

		m_Nodes.Add( new NodeElement( new Node(), "one", Color.red, new Vector2( 100, 50 ) ) );
		m_Nodes.Add( new NodeElement( new Node(), "two", Color.yellow, new Vector2( 200, 50 ) ) );

		foreach( var node in m_Nodes )
		{
			root.Add( node );
		}

		m_Nodes[0].node.ConnectTo( m_Nodes[1].node );
	}

	private void OnGUI()
	{
		foreach( var node in m_Nodes )
		{
			foreach( var edge in node.node.OutEdges )
			{
				DrawEdge( node, edge.dstNode );
			}
		}
	}

	private void DrawEdge( INode startNode, INode endNode )
	{
		var startPos = startNode.GetStartPosition();
		var endPos = endNode.GetEndPosition();
		var startNorm = startNode.GetStartNorm();
		var endNorm = endNode.GetEndNorm();

		Handles.DrawBezier(
		    startPos,
		    endPos,
		    startPos + 0.4f * Vector3.Dot( endPos - startPos, startNorm ) * startNorm,
		    endPos + 0.4f * Vector3.Dot( startPos - endPos, endNorm ) * endNorm,
		    color: Color.blue,
		    texture: null,
		    width: 2f );
	}
}

public interface INode
{
	Vector3 GetStartPosition();
	Vector3 GetEndPosition();
	Vector3 GetStartNorm();
	Vector3 GetEndNorm();
}
