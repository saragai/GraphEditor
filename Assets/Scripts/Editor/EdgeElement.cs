using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class EdgeElement : VisualElement
{
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

    public EdgeElement(NodeElement from, NodeElement to) : this()
    {
        From = from;
        To = to;
    }

    private EdgeElement()
    {
    }

    public void ConnectTo(NodeElement to)
    {
        To = to;
        MarkDirtyRepaint();
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

    private void DrawEdge(INode startNode, INode endNode)
    {
        var endPos = endNode.GetEndPosition();
        var endNorm = endNode.GetEndNorm();

        DrawEdge(startNode, endPos, endNorm);
    }

    private void DrawEdge(INode startNode, Vector2 endPos, Vector2 endNorm)
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
            startPos + Mathf.Max(0.4f * Vector3.Dot(endPos - startPos, startNorm), 50f) * startNorm,
            endPos + Mathf.Max(0.4f * Vector3.Dot(startPos - endPos, endNorm), 50f) * endNorm,
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
