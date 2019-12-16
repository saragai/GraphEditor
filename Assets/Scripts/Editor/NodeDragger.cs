using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class NodeDragger : MouseManipulator
{
    private bool m_Focus;

    public NodeDragger()
    {
        // activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
    }
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
    }

    protected void OnMouseDown(MouseDownEvent evt)
    {
        if(evt.button != 0)
        {
            return;
        }

        m_Focus = true;
        target.BringToFront();
        target.CaptureMouse();
    }

    protected void OnMouseUp(MouseUpEvent evt)
    {
        target.ReleaseMouse();

        if(target is NodeElement node)
        {
            node.SerializableNode.position = target.transform.position;
        }

        m_Focus = false;
    }

    protected void OnMouseMove(MouseMoveEvent evt)
    {
        if (m_Focus)
        {
            target.transform.position += (Vector3)evt.mouseDelta;
        }
    }
}
