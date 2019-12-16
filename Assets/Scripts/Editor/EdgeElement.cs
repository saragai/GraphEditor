using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class EdgeElement : VisualElement
{
    readonly float CURVE_SIZE = 50f;
    readonly float INTERCEPT_WIDHT = 15f;

    public SerializableEdge serializableEdge;

    private Vector2 m_CandidatePosition;
    public Vector2 CandidatePosition
    {
        get { return m_CandidatePosition; }
        set
        {
            m_CandidatePosition = this.WorldToLocal(value);
        }
    }

    public NodeElement From { get; private set; }
    public NodeElement To { get; private set; }

    public EdgeElement(NodeElement from, Vector2 candidatePosition) : this()
    {
        From = from;
        m_CandidatePosition = candidatePosition;
    }

    public EdgeElement(SerializableEdge edge, NodeElement from, NodeElement to) : this()
    {
        serializableEdge = edge;
        From = from;
        To = to;
    }

    private EdgeElement()
    {
        this.AddManipulator(new ContextualMenuManipulator(evt =>
        {
            if (evt.target is EdgeElement)
            {
                evt.menu.AppendAction(
                "Remove Edge",
                (DropdownMenuAction menuItem) =>
                {
                    var graph = GetFirstAncestorOfType<GraphEditorElement>();
                    graph.RemoveEdgeElement(this);
                },
                DropdownMenuAction.AlwaysEnabled);
            }
        }));
    }

    public void ConnectTo(NodeElement to)
    {
        To = to;
        MarkDirtyRepaint();
    }

    public override bool ContainsPoint(Vector2 localPoint)
    {
        if (From == null || To == null)
        {
            return false;
        }

        Vector2 start = From.GetStartPosition();
        Vector2 end = To.GetEndPosition();

        Vector2 rectPos = new Vector2(Mathf.Min(start.x, end.x), Mathf.Min(start.y, end.y));
        Vector2 rectSize = new Vector2(Mathf.Abs(start.x - end.x), Mathf.Abs(start.y - end.y));
        Rect bound = new Rect(rectPos, rectSize);

        if (!bound.Contains(localPoint))
        {
            return false;
        }

        start = From.GetStartPosition() + CURVE_SIZE / 4f * From.GetStartNorm();
        end = To.GetEndPosition() + CURVE_SIZE / 4f * To.GetEndNorm();

        if (start == end)
        {
            return Vector2.Distance(localPoint, start) < INTERCEPT_WIDHT;
        }

        float distance = Mathf.Abs(
            (end.y - start.y) * localPoint.x
            - (end.x - start.x) * localPoint.y
            + end.x * start.y - end.y * start.x
            ) / Vector2.Distance(start, end);

        if (distance < INTERCEPT_WIDHT)
        {
            return true;
        }

        return false;
    }

    public void DrawEdge()
    {
        if (From != null && To != null)
        {
            DrawEdge(From, To);
        }
        else if (From != null)
        {
            DrawEdge(From, m_CandidatePosition, Vector2.zero);
        }
    }

    private void DrawEdge(NodeElement startNode, NodeElement endNode)
    {
        var endPos = endNode.GetEndPosition();
        var endNorm = endNode.GetEndNorm();

        DrawEdge(startNode, endPos, endNorm);
    }

    private void DrawEdge(NodeElement startNode, Vector2 endPos, Vector2 endNorm)
    {
        var startPos = startNode.GetStartPosition();
        var startNorm = startNode.GetStartNorm();

        DrawEdge(startPos, startNorm, endPos, endNorm);
    }

    private void DrawEdge(Vector2 startPos, Vector2 startNorm, Vector2 endPos, Vector2 endNorm)
    {
        Handles.color = Color.blue;
        Handles.DrawBezier(
            startPos,
            endPos,
            startPos + CURVE_SIZE * startNorm,
            endPos + CURVE_SIZE * endNorm,
            color: Color.blue,
            texture: null,
            width: 2f);

        Vector2 arrowAxis = 10f * endNorm;
        Vector2 arrowNorm = 5f * Vector3.Cross(endNorm, Vector3.forward);

        Handles.DrawAAConvexPolygon(endPos,
            endPos + arrowAxis + arrowNorm,
            endPos + arrowAxis - arrowNorm);
        Handles.color = Color.white;
    }
}
