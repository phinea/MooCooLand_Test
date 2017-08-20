using UnityEngine;

namespace MooCooEngine.Input.Cursors
{
    /// <summary>
    /// Manual cursor control, e.g., via a controller or touchpad.
    /// </summary>
    public class Cursor_Manual : CustomCursor
    {
        public Cursor_Manual(string name, CursorType type)
        {
            base.id = name;
            base.CursorType = type;
        }

        public override void Destroy()
        {
            Debug.Log(">> Cursor_Manual > Destroyed.");
        }

        public void SetPosition2D(Vector2 vec)
        {
            base.ScreenPos = vec;
        }

        public void SetPosition3D(Vector3 vec)
        {
            base.SetWorldPoint(vec);
        }
    }
}
