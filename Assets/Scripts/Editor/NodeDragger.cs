using UnityEngine;
using UnityEngine.UIElements;

public class NodeDragger : MouseManipulator
{
    private bool m_Focus;

    public NodeDragger()
    {
        // 左クリックで有効化する
        activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
    }

    /// Manipulatorにターゲットがセットされたときに呼ばれる
    protected override void RegisterCallbacksOnTarget()
    {
        m_Focus = false;

        target.RegisterCallback<MouseDownEvent>(OnMouseDown);
        target.RegisterCallback<MouseUpEvent>(OnMouseUp);
        target.RegisterCallback<MouseMoveEvent>(OnMouseMove);
        target.RegisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
    }

    /// Manipulatorのターゲットが変わる直前に呼ばれる
    protected override void UnregisterCallbacksFromTarget()
    {
        target.UnregisterCallback<MouseDownEvent>(OnMouseDown);
        target.UnregisterCallback<MouseUpEvent>(OnMouseUp);
        target.UnregisterCallback<MouseMoveEvent>(OnMouseMove);
        target.UnregisterCallback<MouseCaptureOutEvent>(OnMouseCaptureOut);
    }

    protected void OnMouseDown(MouseDownEvent evt)
    {
        // 設定した有効化条件をみたすか (= 左クリックか)
        if (CanStartManipulation(evt))
        {
            m_Focus = true;
            target.BringToFront();
            target.CaptureMouse();
        }
    }

    protected void OnMouseUp(MouseUpEvent evt)
    {
        if (CanStopManipulation(evt))
        {
            target.ReleaseMouse();

            if(target is NodeElement node)
            {
                node.serializableNode.position = target.transform.position;
            }

            m_Focus = false;
        }
    }

    protected void OnMouseCaptureOut(MouseCaptureOutEvent evt)
    {
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