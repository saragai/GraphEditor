using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class NodeElement : VisualElement
{
    public NodeElement (string name, Color color, Vector2 pos)
    {
        style.backgroundColor = new StyleColor(color);
        style.position = Position.Absolute;
        style.height = 50;
        style.width = 100;

        transform.position = pos;

        Add(new Label(name));

        this.AddManipulator(new NodeDragger());

        // this.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));
    }

    void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
    {
        evt.menu.AppendAction(
            "Add Edge",  // 項目名
            AddEdgeMenuAction,
            DropdownMenuAction.AlwaysEnabled);
    }

    void AddEdgeMenuAction(DropdownMenuAction menuAction)
    {
        Debug.Log("Add Edge");
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
