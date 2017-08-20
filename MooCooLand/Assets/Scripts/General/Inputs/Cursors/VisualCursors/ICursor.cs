using UnityEngine;

namespace MooCooEngine.Input.Cursors
{
    public class ICursor : MonoBehaviour
    {
        public CursorType inputType;
        private bool initialized = false;
        internal CustomCursor RawInputSignal;

        protected void Update()
        {
            if (!initialized)
            {
                if (InputManager.Instance != null)
                {
                    try
                    {
                        RawInputSignal = InputManager.Instance.GetCursorOfType(inputType);

                        if (RawInputSignal != null)
                        {
                            RawInputSignal.OnCursorUpdated += Cursor_OnCursorUpdated;
                            initialized = true;
                        }
                    }
                    catch (System.NullReferenceException)
                    {
                        //Debug.LogWarning("Cursor not yet initialized.");
                    }
                }
            }
        }

        public void ResetCursor()
        {
            if (RawInputSignal != null)
                RawInputSignal.OnCursorUpdated -= Cursor_OnCursorUpdated;

            initialized = false;
        }

        public virtual void Cursor_OnCursorUpdated(object sender, CursorEventArgs e)
        {
            //Debug.Log(">>CursorUpdate: " + e.Cursor.inputtype);
        }
    }
}