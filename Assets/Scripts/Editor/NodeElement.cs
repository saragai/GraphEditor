using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class NodeElement : Box
{
    private readonly NodeDragger m_Mover;

    public SerializableNode serializableNode;

    public NodeElement(SerializableNode node, string name)
    {
        serializableNode = node;

        style.position = Position.Absolute;
        style.height = 50;
        style.width = 100;
        transform.position = node.position;

        Add(new Label(name));

        this.AddManipulator(new NodeDragger());
        this.AddManipulator(new EdgeConnector());

        this.AddManipulator(new ContextualMenuManipulator(evt =>
        {
            if (evt.target is NodeElement)
            {
                evt.menu.AppendSeparator();
                evt.menu.AppendAction(
                    "Remove Node",
                    menuItem =>
                    {
                        var graph = GetFirstAncestorOfType<GraphEditorElement>();
                        graph.RemoveNodeElement(this);
                    },
                    DropdownMenuAction.AlwaysEnabled);
            }
        }));
    }

    public Vector2 GetStartPosition()
    {
        return (Vector2)transform.position + new Vector2(style.width.value.value / 2f, style.height.value.value);
    }
    public Vector2 GetEndPosition()
    {
        return (Vector2)transform.position + new Vector2(style.width.value.value / 2f, 0f);
    }
    public Vector2 GetStartNorm()
    {
        return new Vector2(0f, 1f);
    }
    public Vector2 GetEndNorm()
    {
        return new Vector2(0f, -1f);
    }
}
