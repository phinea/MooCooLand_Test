using UnityEngine;

namespace MooCooEngine.Input.Cursors
{
    /// <summary>
    /// Meta class for different types of cursors
    /// </summary>
    
    public enum CursorType
    {
        NotDefined,
        Mouse,
        CenteredCam,
        Manual,        
    }
    
    public abstract class CustomCursor
    {
        public CursorType CursorType;
        public string id;
        private Ray pointerRay;
        private Vector3 _screenPos; //# In screen space

        public abstract void Destroy();

        public virtual void Update()
        {
            UpdateCursor(this);
        }

        #region Set methods
        /// <summary>
        /// Cursor position in screen space.
        /// </summary>
        /// <param name="ScreenPos"></param>
        public virtual Vector3 ScreenPos
        {
            internal set
            {
                _screenPos = value;
                pointerRay = Camera.main.ScreenPointToRay(_screenPos);
            }
            get
            {
                return _screenPos;
            }           
        }

        public virtual void SetWorldPoint(Vector3 pnt)
        {
            ScreenPos = Camera.main.WorldToScreenPoint(pnt);
     //       pointerRay = new Ray(Camera.main.transform.position, pnt); // Assuming that the origin of the ray is at the camera center
        }

        public virtual void SetRay(Vector3 origin, Vector3 dir)
        {
            _screenPos = Camera.main.WorldToScreenPoint(origin+dir); // TODO: Check if this works - Better to use destination?
            pointerRay = new Ray(origin, dir);
        } 
        #endregion

        #region Get methods
        public Vector3 Origin
        {
            get
            {
                return PointingRay.origin;
            }
        }

        public Vector3 Direction
        {
            get
            {
                return PointingRay.direction;
            }
        }

        public Ray PointingRay
        {
            get
            {
                return pointerRay;
            }
        }
        #endregion

        #region Custom cursor events
        public delegate void CursorUpdated(object sender, CursorEventArgs e);
        public event CursorUpdated OnCursorUpdated;
        
        private void UpdateCursor(CustomCursor cursor)
        {
            //# Make sure someone is listening to event
            if (OnCursorUpdated != null)
                OnCursorUpdated(this, new CursorEventArgs(cursor));
        }
        #endregion
    }

    public class CursorEventArgs : System.EventArgs
    {
        public CustomCursor Cursor { get; private set; }

        public CursorEventArgs(CustomCursor cursor)
        {
            Cursor = cursor;
        }
    }
}
