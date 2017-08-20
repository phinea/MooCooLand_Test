using UnityEngine;

namespace MooCooEngine.Input.Cursors
{
    /// <summary>
    /// Mouse cursor
    /// </summary>
    public class Cursor_Mouse : CustomCursor
    {
        public Cursor_Mouse(string name)
        {
            base.id = name;
            base.CursorType = CursorType.Mouse;
        }

        public override void Destroy()
        {
            Debug.Log(">> Cursor_Mouse > Destroyed.");
        }

        public override void Update()
        {
            Ray mousePos = Camera.main.ScreenPointToRay(UnityEngine.Input.mousePosition);
            base.SetRay(mousePos.origin, mousePos.direction);
            base.Update();
        }
    }
}
