﻿using UnityEngine;
using UnityEngine.UIElements;

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
        this.AddManipulator(new EdgeConnector());
        this.AddManipulator(new ContextualMenuManipulator(OnContextualMenuPopulate));
    }

    private void OnContextualMenuPopulate(ContextualMenuPopulateEvent evt)
    {
        if (evt.target is NodeElement)
        {
            evt.menu.AppendAction(
                "Remove Node",
                RemoveNodeMenuAction,
                DropdownMenuAction.AlwaysEnabled);
        }
    }

    private void RemoveNodeMenuAction(DropdownMenuAction menuAction)
    {
        var graph = GetFirstAncestorOfType<GraphEditorElement>();
        graph.RemoveNodeElement(this);
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
