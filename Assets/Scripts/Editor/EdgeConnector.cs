using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class EdgeConnector: MouseManipulator
{
    bool m_Active = false;
    EdgeElement m_ConnectingEdge;

    GraphEditorElement m_Graph;

    ContextualMenuManipulator m_AddEdgeMenu;

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
        m_Graph = target.GetFirstAncestorOfType<GraphEditorElement>();

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

        m_Graph = null;
    }

    protected void OnMouseDown(MouseDownEvent evt)
    {
        if (m_Active)
        {
            evt.StopImmediatePropagation();
            return;
        }

        if (!CanStartManipulation(evt))
        {
            return;
        }
    }

    protected void OnMouseUp(MouseUpEvent evt)
    {
        if (!m_Active)
            return;

        if (!CanStopManipulation(evt))
        {
            return;
        }

        var node = GetDesignatedNode(evt.originalMousePosition);

        if (node == null || CheckOverlapping(node))
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
        if (m_Active)
        {
            m_ConnectingEdge.CandidatePosition = evt.originalMousePosition;
            m_ConnectingEdge.MarkDirtyRepaint();
        }
    }

    private void OnCaptureOut(MouseCaptureOutEvent evt)
    {
        if (m_Active)
        {
            Abort();
        }
    }

    private NodeElement GetDesignatedNode(Vector2 position)
    {
        foreach (NodeElement node in m_Graph.Query<NodeElement>().Build().ToList())
        {
            if (node == target)
                continue;

            if (node.worldBound.Contains(position))
            {
                return node;
            }
        }

        return null;
    }

    private bool CheckOverlapping(NodeElement toNode)
    {
        return m_Graph.SameEdgeExists(m_ConnectingEdge.From, toNode);
    }

    private void Abort()
    {
        m_Graph.RemoveEdgeElement(m_ConnectingEdge);
        m_Active = false;
        m_ConnectingEdge = null;
        target.ReleaseMouse();
    }
}
