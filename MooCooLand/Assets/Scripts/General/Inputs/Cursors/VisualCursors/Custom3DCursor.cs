using UnityEngine;

namespace MooCooEngine.Input.Cursors
{
    public enum CursorBehaviorsOnTarget
    {
        Continuous, // Default: Continuously display cursor at input signal
        SnapToTargetCenter, // If over a target, snap to the target center
        MoveToTargetCenter, // If over a target, slowly move the cursor to anchor at the target center
        HideAndHighlightTarget, // If over a target, blend out the cursor and highlight the target
        ManualOverride
    }

    public class Custom3DCursor : ICursor
    {
        public GameObject CustomCursor;

        [Tooltip("Max distance to which the cursor is scaled. If the distance is larger than MaxCursorDistance, the cursor remains at the same size.")]
        public float MaxCursorDistance = 5f;
        private float currDistCamToCursor;
        public bool useConstantDistance = true;

        public CursorBehaviorsOnTarget CursorBehavior;
        private bool? CursorIsVisible;
        internal bool ShowCursor
        {
            get {
                if (CursorIsVisible.HasValue)
                    return CursorIsVisible.Value;
                else
                    return false;
            }
            set
            {
                if (!CursorIsVisible.HasValue || (CursorIsVisible.Value != value))
                {
                    UpdateCursor();
                    CursorIsVisible = value;

                    if (CursorIsVisible.Value)
                    {
                        SetTransparency = OrigTransparency;
                        CustomCursor.SetActive(CursorIsVisible.Value);
                    }
                    else
                        SetTransparency = 0;
                }
                // CustomCursor.SetActive(CursorIsVisible);
            }
        }
        internal bool ShowHighlights = false;
        public float TransitionSpeed = 0.2f;

        internal Vector3 CursorPos;
        internal Vector3 CursorDir;

        // TODO: Update transparency in shader
        public float OrigTransparency = 1;
        internal float Transparency = 1;

        void Start()
        {
            currDistCamToCursor = MaxCursorDistance;
        }

        new void Update()
        {
            try
            {
                UpdateCursor();
                base.Update();
            }
            catch (System.NullReferenceException) { Debug.LogWarning("GUI not yet initialized."); }
        }



        private void UpdateCursor()
        {
            if ((CustomCursor != null) && (RawInputSignal != null))
            {
                //// Place cursor
                //CustomCursor.transform.position = RawCursorPosition;

                //// Cursor should always face the camera
               // CustomCursor.transform.rotation = Quaternion.FromToRotation(Vector3.forward, InputManager.Instance.GetCursorOfType(inputType).Direction);

                if (!useConstantDistance)
                {
                    if (InputManager.Instance.Hit)
                    {
                        float distCam2Target = (Camera.main.transform.position - InputManager.Instance.HitPosition).magnitude/2;
                        if (distCam2Target > MaxCursorDistance)
                            currDistCamToCursor = MaxCursorDistance;
                        else
                            currDistCamToCursor = distCam2Target;
                    }
                    else
                    {
                        currDistCamToCursor = MaxCursorDistance;
                    }
                }
                else
                {
                    currDistCamToCursor = MaxCursorDistance;
                }

                UpdateCursorBehavior();

                // Place cursor
                CustomCursor.transform.position = CursorPos;

                // Cursor should always face the camera
                // CustomCursor.transform.rotation = Quaternion.FromToRotation(Vector3.forward, CursorDir);

                // Set visibility
                // CustomCursor.SetActive(ShowCursor);
            }
        }
        
        public void UpdateCursorBehavior()
        {
             switch (CursorBehavior)
            {
                case CursorBehaviorsOnTarget.Continuous:
                    SetContinuousCursor();
                    break;
                case CursorBehaviorsOnTarget.SnapToTargetCenter:
                    if (InputManager.Instance.Hit)
                    {
                        CursorDir = InputManager.Instance.HitTarget.transform.position - Camera.main.transform.position;
                        CursorPos = Camera.main.transform.position + CursorDir.normalized * currDistCamToCursor;
                    }
                    else
                    {
                        SetContinuousCursor();
                    }
                    break;
                case CursorBehaviorsOnTarget.MoveToTargetCenter:
                    if (InputManager.Instance.Hit)
                    {
                        CursorDir = InputManager.Instance.HitTarget.transform.position - Camera.main.transform.position;
                        Vector3 TargetCursorPos = Camera.main.transform.position + CursorDir.normalized * currDistCamToCursor;

                        Vector3 cursorMovingDir = CursorPos - TargetCursorPos;
                        CursorPos = CursorPos - (cursorMovingDir) * TransitionSpeed; // TODO: Make adjustable in editor; Would be nicer to have a more controllable speed? Currently speed is depending on distance to the target
                    }
                    else
                    {
                        SetContinuousCursor();
                    }
                    break;
                case CursorBehaviorsOnTarget.HideAndHighlightTarget:
                    if (InputManager.Instance.Hit)
                    {
                        ShowCursor = false;
                    }
                    else
                    {
                        ShowCursor = true;
                    }

                    SetContinuousCursor();
                    break;
                case CursorBehaviorsOnTarget.ManualOverride:
                    SetContinuousCursor();
                    break;
            }
        }

        public float SetTransparency
        {
            set
            {
                if ((value >= 0) && (value <= 1))
                {
                    Transparency = value;
                    UpdateTransparency(Transparency);
                }
            }
        }

        private void UpdateTransparency(float transparency)
        {
            try
            {
                Renderer[] renderers = this.GetComponents<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    Material[] mats = renderers[i].materials;
                    foreach (Material mat in mats)
                    {
                        Color c = mat.color;
                        mat.color = new Color(c.r, c.g, c.b, transparency);
                    }
                }

                renderers = this.GetComponentsInChildren<Renderer>();
                for (int i = 0; i < renderers.Length; i++)
                {
                    Material[] mats = renderers[i].materials;
                    foreach (Material mat in mats)
                    {
                        Color c = mat.color;
                        mat.color = new Color(c.r, c.g, c.b, transparency);
                    }
                }
            }
            catch (MissingReferenceException)
            {
                // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
            }
        }



        public void ManualOverride_SetPosition()
        {
            CursorDir = InputManager.Instance.HitTarget.transform.position - Camera.main.transform.position;
            CursorPos = Camera.main.transform.position + CursorDir.normalized * currDistCamToCursor;
        }

        private void SetContinuousCursor()
        {
            if (useConstantDistance)
            {
                CursorPos = GetCursorPosition();
                CursorDir = RawInputSignal.Direction;
            }
            else
            {
                if (InputManager.Instance.Hit)
                {
                    if(Vector3.Distance(Camera.main.transform.position, InputManager.Instance.HitPosition) > MaxCursorDistance)
                        CursorPos = Camera.main.transform.position + CursorDir.normalized * MaxCursorDistance; 
                    else
                        //CursorPos = Camera.main.transform.position + CursorDir.normalized * currDistCamToCursor;
                        CursorPos = InputManager.Instance.HitPosition - InputManager.Instance.MainCursor.Direction.normalized * 0.1f;
                }
                else
                    CursorPos = GetCursorPosition(); //Camera.M - InputManager.Instance.MainCursor.Direction.normalized * 0.1f;
                CursorDir = RawInputSignal.Direction; //InputManager.Instance.HitNormal;
            }   
        }



        internal Vector3 GetCursorPosition()
        {
            // Here we can also return offset corrected cursor positions
            //if (useConstOffset)
            //{
            //    return OffsetCursor();
            //}
            //else
            //{
            //    return cursor.pos;
            //}
            return Camera.main.transform.position + CursorDir.normalized * currDistCamToCursor;
            //return RawCursorPosition;
        }

        internal Vector3 RawCursorPosition
        {
            get
            {
                if (RawInputSignal != null)
                    return (RawInputSignal.Origin + RawInputSignal.Direction.normalized * currDistCamToCursor);
                else
                    return Vector3.zero;
            }
        }

        public Vector3 OriginRaw
        {
            get { return RawInputSignal.Origin; }
        }

        public Vector3 DirectionRaw
        {
            get { return RawInputSignal.Direction; }
        }
    }
}
