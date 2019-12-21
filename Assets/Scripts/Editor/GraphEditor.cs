using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine.UIElements;

public class GraphEditor : EditorWindow
{
    [MenuItem("Window/GraphEditor")]
    public static void ShowWindow()
    {
        GraphEditor graphEditor = CreateInstance<GraphEditor>();
        graphEditor.Show();
        graphEditor.titleContent = new GUIContent("Graph Editor");

        if(Selection.activeObject is GraphAsset graphAsset)
        {
            graphEditor.Initialize(graphAsset);
        }
    }

    [OnOpenAsset()]
    private static bool OnOpenAsset(int instanceId, int line)
    {
        if(EditorUtility.InstanceIDToObject(instanceId) is GraphAsset)
        {
            ShowWindow();
            return true;
        }

        return false;
    }

    GraphAsset m_GraphAsset;
    GraphEditorElement m_GraphEditorElement;

    public void OnEnable()
    {
        if (m_GraphAsset != null)
        {
            Initialize(m_GraphAsset);
        }
    }

    public void Initialize(GraphAsset graphAsset)
    {
        m_GraphAsset = graphAsset;

        VisualElement root = this.rootVisualElement;

        m_GraphEditorElement = new GraphEditorElement(graphAsset);
        root.Add(m_GraphEditorElement);
    }

    private void OnGUI()
    {
        if(m_GraphEditorElement == null)
        {
            return;
        }

        m_GraphEditorElement.DrawEdge();
    }
}
