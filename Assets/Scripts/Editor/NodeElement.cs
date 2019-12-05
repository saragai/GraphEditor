using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class NodeElement : VisualElement, INode
{
    public Color color;
    public Vector2 position;

    private readonly Mover m_Mover;

    public Node node;

    public NodeElement (Node node,string name, Color color, Vector2 pos)
    {
        style.backgroundColor = new StyleColor(color);
        style.position = Position.Absolute;
        style.height = 50;
        style.width = 100;

        this.node = node;

        transform.position = pos;

        m_Mover = new Mover(this);

        Add(new Label(name));

        new ContextualMenuManipulator((ContextualMenuPopulateEvent evt) =>
        {
            evt.menu.AppendAction(
                "Add Edge",
                (DropdownMenuAction menuItem) =>
                {
                    Debug.Log("Add Edge");
                },
                DropdownMenuAction.AlwaysEnabled);
        })
        {
            target = this
        };
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

public class Mover
{
    private bool m_Focus;
    private VisualElement m_Owner;

    public Mover (VisualElement visualElement)
    {
        m_Owner = visualElement;

        m_Owner.RegisterCallback<MouseDownEvent>(MouseDownCallback);
        m_Owner.RegisterCallback<MouseUpEvent>(MouseUpCallback);
        m_Owner.RegisterCallback<MouseMoveEvent>(MouseMoveCallback);
    }

    public void MouseDownCallback(MouseDownEvent evt)
    {
        if (evt.button == 0)
        {
            m_Focus = true;
            m_Owner.BringToFront();
            m_Owner.CaptureMouse();
        }
    }

    public void MouseUpCallback(MouseUpEvent evt)
    {
        switch (evt.button)
        {
            case 0:
                m_Owner.ReleaseMouse();
                m_Focus = false;
                break;
        }
    }

    public void MouseMoveCallback(MouseMoveEvent evt)
    {
        if (m_Focus)
        {
            m_Owner.transform.position += (Vector3)evt.mouseDelta;
        }
    }
}

