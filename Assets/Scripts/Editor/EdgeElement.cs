using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;

public class EdgeElement : VisualElement
{
    public SerializableEdge serializableEdge;

    public NodeElement From { get; private set; }
    public NodeElement To { get; private set; }

    Vector2 m_ToPosition;
    public Vector2 ToPosition
    {
        get { return m_ToPosition; }
        set
        {
            m_ToPosition = value;
            MarkDirtyRepaint();
        }
    }

    public EdgeElement()
    {
        style.position = Position.Absolute;

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
                    MarkDirtyRepaint();
                },
                DropdownMenuAction.AlwaysEnabled);
            }
        }));
    }

    public EdgeElement(NodeElement fromNode, Vector2 toPosition):this()
    {
        From = fromNode;
        ToPosition = toPosition;
    }

    public EdgeElement(SerializableEdge edge, NodeElement fromNode, NodeElement toNode):this()
    {
        serializableEdge = edge;
        From = fromNode;
        To = toNode;
    }

    public void ConnectTo(NodeElement node)
    {
        To = node;
        MarkDirtyRepaint();
    }

    readonly float INTERCEPT_WIDHT = 15f;

    public override bool ContainsPoint(Vector2 localPoint)
    {
        if (From == null || To == null)
            return false;

        if (!base.ContainsPoint(localPoint))
        {
            return false;
        }

        localPoint = this.ChangeCoordinatesTo(parent, localPoint);

        Vector2 a = From.GetStartPosition() + 12f * From.GetStartNorm();
        Vector2 b = To.GetEndPosition() + 12f * To.GetEndNorm();

        if (a == b)
        {
            return Vector2.Distance(localPoint, a) < INTERCEPT_WIDHT;
        }

        float distance = Mathf.Abs(
            (b.y - a.y) * localPoint.x
            - (b.x - a.x) * localPoint.y
            + b.x * a.y - b.y * a.x
            ) / Vector2.Distance(a, b);

        return distance < INTERCEPT_WIDHT;
    }

    private Rect GetBoundingBox()
    {
        Vector2 start = From.GetStartPosition();
        Vector2 end = To.GetEndPosition();

        Vector2 rectPos = new Vector2(Mathf.Min(start.x, end.x) - 12f, Mathf.Min(start.y, end.y) - 12f);
        Vector2 rectSize = new Vector2(Mathf.Abs(start.x - end.x) + 24f, Mathf.Abs(start.y - end.y) + 24f);
        Rect bound = new Rect(rectPos, rectSize);

        return bound;
    }

    private void UpdateLayout()
    {
        Rect bound = GetBoundingBox();

        style.left = bound.x;
        style.top = bound.y;
        style.right = float.NaN;
        style.bottom = float.NaN;
        style.width = bound.width;
        style.height = bound.height;
    }

    public void DrawEdge()
    {
        if (From != null && To != null)
        {
            UpdateLayout();

            DrawEdge(
                startPos: From.GetStartPosition(),
                startNorm: From.GetStartNorm(),
                endPos: To.GetEndPosition(),
                endNorm: To.GetEndNorm());
        }
        else
        {
            if (From != null)
            {
                DrawEdge(
                    startPos: From.GetStartPosition(),
                    startNorm: From.GetStartNorm(),
                    endPos: ToPosition,
                    endNorm: Vector2.zero);
            }
        }
    }

    private void DrawEdge(Vector2 startPos, Vector2 startNorm, Vector2 endPos, Vector2 endNorm)
    {
        Handles.color = Color.blue;
        Handles.DrawBezier(
            startPos,
            endPos,
            startPos + 50f * startNorm,
            endPos + 50f * endNorm,
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
