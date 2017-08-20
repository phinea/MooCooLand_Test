using UnityEngine;

namespace MooCooEngine.Input.Cursors
{
    /// <summary>
    /// Cursor that is centered at the displayed camera view.
    /// </summary>
    public class Cursor_CenteredCam : CustomCursor
    {
        public Cursor_CenteredCam(string name)
        {
            base.id = name;
            base.CursorType = CursorType.CenteredCam;
        }
        public override void Destroy()
        {
            Debug.Log(">> Cursor_CenteredCam > Destroyed.");
        }

        public override void Update()
        {
            base.SetRay(Camera.main.transform.position, Camera.main.transform.forward);
            base.Update();
        }
    }
}