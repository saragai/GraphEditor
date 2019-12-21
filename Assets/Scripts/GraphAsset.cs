using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="graph.asset", menuName ="Graph Asset")]
public class GraphAsset : ScriptableObject
{
    public List<SerializableNode> nodes = new List<SerializableNode>();
}

[System.Serializable]
public class SerializableNode
{
    public Vector2 position;
    public List<SerializableEdge> edges = new List<SerializableEdge>();
}

[System.Serializable]
public class SerializableEdge
{
    public int toId;
}
