using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;

public class GraphEditor : EditorWindow
{
    [MenuItem("Window/GraphEditor")]
    public static void ShowWindow()
    {
        GraphEditor graphEditor = GetWindow<GraphEditor>();
        graphEditor.titleContent = new GUIContent("Graph Editor");
    }

    List<NodeElement> m_Nodes = new List<NodeElement>();

    public void OnEnable()
    {
        VisualElement root = this.rootVisualElement;

        root.Add(new NodeElement("One", Color.red, new Vector2(100, 50)));
        root.Add(new NodeElement("Two", Color.yellow, new Vector2(200, 50)));

        root.AddManipulator(new ContextualMenuManipulator(OnContextMenuPopulate));
    }

    void OnContextMenuPopulate(ContextualMenuPopulateEvent evt)
    {
        // 項目を追加
        evt.menu.AppendAction(
            "Add Node",  // 項目名
            AddEdgeMenuAction,  // 選択時の挙動
            DropdownMenuAction.AlwaysEnabled  // 選択可能かどうか
            );
    }

    void AddEdgeMenuAction(DropdownMenuAction menuAction)
    {
        Debug.Log("Add Node");
    }

    private void OnGUI()
    {
    }

    private void DrawEdge(NodeElement startNode, NodeElement endNode)
    {
        var startPos = startNode.GetStartPosition();
        var endPos = endNode.GetEndPosition();
        var startNorm = startNode.GetStartNorm();
        var endNorm = endNode.GetEndNorm();

        Handles.DrawBezier(
            startPos,
            endPos,
            startPos + 0.4f * Vector3.Dot(endPos - startPos, startNorm) * startNorm,
            endPos + 0.4f * Vector3.Dot(startPos - endPos, endNorm) * endNorm,
            color: Color.blue,
            texture: null,
            width: 2f);
    }
}
