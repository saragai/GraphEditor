using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu( fileName = "graph.graph.asset", menuName = "Graph/Graph" )]
public class GraphAsset: ScriptableObject
{
	public List<SerializableNode> nodes = new List<SerializableNode>();
}

[Serializable]
public class SerializableNode
{
	public Vector2 position;
	public List<SerializableEdge> edges;
}

[Serializable]
public class SerializableEdge
{
	public int nodeId;
}
