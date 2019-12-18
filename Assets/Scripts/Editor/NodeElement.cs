using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class NodeElement : Box
{
    public SerializableNode serializableNode;
    public NodeElement (SerializableNode node)
    {
        serializableNode = node;

        style.position = Position.Absolute;
        style.height = 50;
        style.width = 100;

        transform.position = node.position;

        this.AddManipulator(new NodeDragger());
    }


    public Vector3 GetStartPosition()
    {
        return transform.position + new Vector3(style.width.value.value / 2f, style.height.value.value);
    }
    public Vector3 GetEndPosition()
    {
        return transform.position + new Vector3(style.width.value.value / 2f, 0f);
    }
    public Vector3 GetStartNorm()
    {
        return new Vector3(0f, 1f);
    }
    public Vector3 GetEndNorm()
    {
        return new Vector3(0f, -1f);
    }
}
