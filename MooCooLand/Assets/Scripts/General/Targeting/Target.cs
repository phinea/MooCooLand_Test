using UnityEngine;
using MooCooEngine.Utils;
using MooCooEngine.Input;
using MooCooEngine.Input.Cursors;

namespace MooCooEngine
{
    public class Target : DispatcherBehavior //TODO: WHY DISPATCHER??
    {
        private bool isHovered = false;
        private string fullTargetName;

        //# Use this for initialization
        protected override void Start()
        {
            base.Start();

            //# Name 
            fullTargetName = Utils.General.GetFullName(this.gameObject);

            //# Dummy listeners
            this.OnCursorEnter += Target_OnCursorEnter;
            this.OnCursorLeave += Target_OnCursorLeave;
        }

        // Update is called once per frame
        protected override void Update()
        {
            UpdateTargetHighlights();
        }

        public void OnSelect()
        {
            Debug.Log("TargetOnSelect");
            UpdateOnTargetSelected(InputManager.Instance.MainCursor);
        }

        void UpdateTargetHighlights()
        {
            if ((InputManager.Instance != null) && (InputManager.Instance.MainCursor != null))
            {
                if (InputManager.Instance.HitTargetName == fullTargetName)
                {
                    if (!isHovered)
                    {
                        UpdateOnCursorEnter(InputManager.Instance.MainCursor);
                        isHovered = true;
                    }
                    else
                    {
                        UpdateOnCursorMove(InputManager.Instance.MainCursor);
                    }
                }
                else
                {
                    if (isHovered)
                    {
                        UpdateOnCursorLeave(InputManager.Instance.MainCursor);
                        isHovered = false;
                    }
                }
            }
        }

        private void Target_OnCursorEnter(object sender, TargetEventArgs e)
        {
            // Just a dummy
        }

        private void Target_OnCursorLeave(object sender, TargetEventArgs e)
        {
            // Just a dummy
        }

        // Declare custom Event Handler
        public delegate void TargetUpdateHandler(object sender, TargetEventArgs e);
        public event TargetUpdateHandler OnTargetSelected;
        public event TargetUpdateHandler OnCursorEnter;
        public event TargetUpdateHandler OnCursorMove;
        public event TargetUpdateHandler OnCursorLeave;

        private void UpdateOnTargetSelected(CustomCursor cursor)
        {
            // Make sure someone is listening to event
            if (OnTargetSelected != null)
                OnTargetSelected(this, new TargetEventArgs(cursor, this));
        }

        private void UpdateOnCursorEnter(CustomCursor cursor)
        {
            // Make sure someone is listening to event
            if (OnCursorEnter != null)
                OnCursorEnter(this, new TargetEventArgs(cursor, this));
        }

        private void UpdateOnCursorMove(CustomCursor cursor)
        {
            // Make sure someone is listening to event
            if (OnCursorMove != null)
                OnCursorMove(this, new TargetEventArgs(cursor, this));
        }

        private void UpdateOnCursorLeave(CustomCursor cursor)
        {
            // Make sure someone is listening to event
            if (OnCursorLeave != null)
                OnCursorLeave(this, new TargetEventArgs(cursor, this));
        }
    }

    public class TargetEventArgs : System.EventArgs
    {
        public CustomCursor Cursor { get; private set; }
        public Target HitTarget { get; private set; }

        public TargetEventArgs(CustomCursor cursor, Target hitTarget)
        {
            Cursor = cursor;
            HitTarget = hitTarget;
        }
    }
}