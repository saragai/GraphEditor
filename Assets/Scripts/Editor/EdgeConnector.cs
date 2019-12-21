using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EdgeConnector : MouseManipulator
{
    bool m_Active = false;

    ContextualMenuManipulator m_AddEdgeMenu;

    GraphEditorElement m_Graph;
    EdgeElement m_ConnectingEdge;

    public EdgeConnector()
    {
        activators.Add(new ManipulatorActivationFilter() { button = MouseButton.LeftMouse });

        m_Active = false;

        m_AddEdgeMenu = new ContextualMenuManipulator(OnContextualMenuPopulate);
    }

    private void OnContextualMenuPopulate(ContextualMenuPopulateEvent evt)
    {
        if (evt.target is NodeElement node)
        {
            if (!node.ContainsPoint(node.WorldToLocal(evt.mousePosition)))
            {
                evt.StopImmediatePropagation();
                return;
            }

            evt.menu.AppendAction(
                "Add Edge",
                (DropdownMenuAction menuItem) =>
                {
                    m_Active = true;

                    m_Graph = target.GetFirstAncestorOfType<GraphEditorElement>();
                    m_ConnectingEdge = m_Graph.CreateEdgeElement(node, menuItem.eventInfo.mousePosition);

                    target.CaptureMouse();
                },
                DropdownMenuAction.AlwaysEnabled);
        }
    }

    protected override void RegisterCallbacksOnTarget()
    {
        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseCaptureOutEvent>(OnCaptureOut);

        target.AddManipulator(m_AddEdgeMenu);
    }

    protected override void UnregisterCallbacksFromTarget()
    {
        target.RemoveManipulator(m_AddEdgeMenu);

        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseCaptureOutEvent>(OnCaptureOut);
    }

    protected void OnMouseDown(MouseDownEvent evt)
    {
        if (!CanStartManipulation(evt))
            return;

        if (m_Active)
            evt.StopImmediatePropagation();
    }

    protected void OnMouseUp(MouseUpEvent evt)
    {
        if (!CanStopManipulation(evt))
            return;

        if (!m_Active)
            return;

        var node = m_Graph.GetDesignatedNode(evt.originalMousePosition);

        if (node == null
            || node == target
            || m_Graph.ContainsEdge(m_ConnectingEdge.From, node))
        {
            m_Graph.RemoveEdgeElement(m_ConnectingEdge);
        }
        else
        {
            m_ConnectingEdge.ConnectTo(node);
            m_Graph.SerializeEdge(m_ConnectingEdge);
        }

        m_Active = false;
        m_ConnectingEdge = null;
        target.ReleaseMouse();
    }

    protected void OnMouseMove(MouseMoveEvent evt)
    {
        if (!m_Active)
        {
            return;
        }

        m_ConnectingEdge.ToPosition = evt.originalMousePosition;
    }

    private void OnCaptureOut(MouseCaptureOutEvent evt)
    {
        if (!m_Active)
            return;

        m_Graph.RemoveEdgeElement(m_ConnectingEdge);
        m_Active = false;
        m_ConnectingEdge = null;
        target.ReleaseMouse();
    }
}
