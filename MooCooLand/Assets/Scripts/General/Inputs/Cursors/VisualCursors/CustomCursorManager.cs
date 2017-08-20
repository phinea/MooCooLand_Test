using UnityEngine;
using MooCooEngine.Input;

namespace MooCooEngine.Input.Cursors
{
    public class CustomCursorManager : MonoBehaviour, IInputCommandListener
    {
        public CursorType startWithThisCursorType;
        public Custom3DCursor Cursor_Mouse;
        public Custom3DCursor Cursor_Centered;
        public Custom3DCursor Cursor_Manual;
        
        private bool initialized = false;

        // Use this for initialization
        void Start()
        {
            initialized = false;
        }

        void OnGUI()
        {
            if (!initialized)
            {
                RegisterManualInputListener();
                currentCursorType = startWithThisCursorType;

                // Init all cursors
                InitAllCursors();

                // Set default 
                if(currentCursorType != CursorType.NotDefined)
                    UpdateCursor(currentCursorType);

                initialized = true;
            }
        }

        #region Set individual cursors
        void SetToMouse()
        {
            SetCursorVisibility(true, false, false);
            InputManager.Instance.ChangeMainCursor(CursorType.Mouse);
            //InputManager.Instance.ResetInputTypes(new InputType[3] { InputType.Mouse, InputType.Gaze, InputType.CenteredCursor });
        }

        void SetToManual()
        {
            SetCursorVisibility(false, false, true);
            InputManager.Instance.ChangeMainCursor(CursorType.Manual);
            //InputManager.Instance.ResetInputTypes(new InputType[3] { InputType.Mouse, InputType.Gaze, InputType.CenteredCursor });
        }

        void SetToCentered()
        {
            SetCursorVisibility(false, true, false);
            InputManager.Instance.ChangeMainCursor(CursorType.CenteredCam);
            //InputManager.Instance.ResetInputTypes(new InputType[3] { InputType.Mouse, InputType.Gaze, InputType.CenteredCursor });
        }
        #endregion

        #region Handle manual input
        void RegisterManualInputListener()
        {
            // Register for manual input events
            try
            {
                InputMonitor.Instance.Register(this as IInputCommandListener, InputProviderType.KeyboardAll);
            }
            catch (System.NullReferenceException) { }
        }

        void OnDestroy()
        {
            //InputMonitor.Instance.Deregister(this as IInputCommandListener);
            ResetAllCursors();
        }

        private CursorType currentCursorType = CursorType.NotDefined;
        public void InputArrived(object sender, InputCommandArgs args)
        {
            CursorType cm = CursorType.NotDefined;

            switch (args.Action)
            {
                case InputCommand.Cursor_Mouse:
                    cm = CursorType.Mouse;
                    break;
                case InputCommand.Cursor_Manual:
                    cm = CursorType.Manual;
                    break;
                case InputCommand.Cursor_CenteredCam:
                    cm = CursorType.CenteredCam;
                    break;
            }

            if ((cm != CursorType.NotDefined) && (cm != currentCursorType))
            {
                currentCursorType = cm;
                UpdateCursor(cm);
            }
        }

        public void UpdateCursor(CursorType cmode)
        {
            switch (cmode)
            {
                case CursorType.Mouse:
                    SetToMouse();
                    break;
                case CursorType.CenteredCam:
                    SetToCentered();
                    break;
                case CursorType.Manual:
                    SetToManual();
                    break;
            }
        }

        private void SetCursorVisibility(bool cmouse, bool ccentered, bool cmanual)
        {
            ResetAllCursors();

            Cursor_Mouse.gameObject.SetActive(cmouse);
            Cursor_Centered.gameObject.SetActive(ccentered);
            Cursor_Manual.gameObject.SetActive(cmanual);
        }

        private void ResetAllCursors()
        {
            // Reset individual cursors
            Cursor_Mouse.ResetCursor();
            Cursor_Centered.ResetCursor();
            Cursor_Manual.ResetCursor();

            Cursor_Mouse.gameObject.SetActive(false);
            Cursor_Centered.gameObject.SetActive(false);
            Cursor_Manual.gameObject.SetActive(false);
        }

        private void InitAllCursors()
        {
            SetToMouse();
            SetToManual();
            SetToCentered();
        }
        #endregion
    }
} 
