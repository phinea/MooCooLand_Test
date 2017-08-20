using UnityEngine;
using MooCooEngine.Input.Cursors;

namespace MooCooEngine.Input
{
    public class InputManager : Singleton<InputManager>, IInputCommandListener
    {
        public CursorType[] cursorTypes;
        private CustomCursor[] cursors;
        private int mainCursorIndex = 0;

        // Use this for initialization
        void Start()
        {
            RegisterManualInputListener();

            //# Define as singleton instance
            InputManager.Instance = this;
            
            //# Init the to-be-supported cursors
            InitCursors();
        }

        public void ResetCursorTypes(CursorType[] newInputOrder)
        {
            //# Let's reset by destroying all the cursors
            foreach (CustomCursor cursor in cursors)
            {
                if (cursor != null)
                    cursor.Destroy();
            }

            //# The order is important for prioritization
            cursorTypes = new CursorType[newInputOrder.Length];
            cursorTypes = newInputOrder;
            InitCursors();
        }

        /// <summary>
        /// Initialize all cursors
        /// </summary>
        void InitCursors()
        {
            if ((cursorTypes != null) && (cursorTypes.Length > 0))
            {
                cursors = new CustomCursor[cursorTypes.Length];
                UpdateCursors();
            }
        }

        /// <summary>
        /// Update all cursors
        /// </summary>
        void UpdateCursors()
        {
            for (int i = 0; i < cursorTypes.Length; i++)
            {
                //# Easy to also add more cursor inputs here
                switch (cursorTypes[i])
                {
                    case (CursorType.Mouse):
                        cursors[i] = new Cursor_Mouse(name);
                        break;
                    case (CursorType.CenteredCam):
                        cursors[i] = new Cursor_CenteredCam(name);
                        break;
                    case (CursorType.Manual):
                        cursors[i] = new Cursor_Manual(name, CursorType.Manual);
                        break;
                    default:
                        cursors[i] = null;
                        break;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {
            foreach (CustomCursor cursor in cursors)
            {
                if (cursor != null)
                {
                    cursor.Update();
                }
                else
                    InitCursors();
            }

            PerformHitTest();
        }

        public CustomCursor GetCursorOfType(CursorType type)
        {
            foreach (CustomCursor cursor in cursors)
            {
                if (cursor.CursorType == type)
                {
                    return cursor;
                }
            }
            return null;
        }

        public CustomCursor MainCursor
        {
            get
            {
                if ((cursors != null) && (cursors.Length > 0) && (mainCursorIndex >= 0) && (mainCursorIndex < cursors.Length))
                    return cursors[mainCursorIndex];
                else
                    return null;
            }
        }

        public bool ChangeMainCursor(CursorType requestedCursorType)
        {
            for (int i = 0; i < cursorTypes.Length; i++)
            {
                if (requestedCursorType == cursorTypes[i])
                {
                    Debug.Log(">> Change cursor to: " + requestedCursorType);
                    mainCursorIndex = i;
                    return true;
                }
            }
            return false;
        }

        #region Hit test handling
        private float HitLastDistance;

        /// <summary>
        /// Physics.Raycast result is true if it hits a hologram.
        /// </summary>
        public bool Hit { get; private set; }

        /// <summary>
        /// Name of the current hit target.
        /// </summary>
        public string HitTargetName
        {
            get; private set;
        }
        
        /// <summary>
        /// HitInfo property gives access
        /// to RaycastHit public members.
        /// </summary>
        public RaycastHit HitInfo { get; private set; }

        /// <summary>
        /// Position of the intersection of the user's gaze and the holograms in the scene.
        /// </summary>
        public Vector3 HitPosition { get; private set; }

        /// <summary>
        /// Raycast hit normal direction.
        /// </summary>
        public Vector3 HitNormal { get; private set; }

        /// <summary>
        /// Game object of the current hit target.
        /// </summary>
        public GameObject HitTarget
        {
            get; private set;
        }

        void PerformHitTest()
        {
            if (MainCursor != null)
            {
                RaycastHit hitInfo = new RaycastHit();
                Hit = Physics.Raycast(MainCursor.PointingRay, out hitInfo);

                if (Hit)
                {
                    HitTarget = hitInfo.collider.gameObject;
                    HitPosition = hitInfo.point;
                    HitNormal = hitInfo.normal;
                    HitLastDistance = hitInfo.distance;
                    HitTargetName = Utils.General.GetFullName(HitTarget);
                }
                else
                {
                    HitTarget = null;
                    HitTargetName = "";
                }
            }
        }
        #endregion

        #region Handle input commands
        void OnDestroy()
        {
            Debug.Log("Try to deregister as input listener");
            InputMonitor.Instance.Deregister(this as IInputCommandListener);
        }

        void RegisterManualInputListener()
        {
            // Register for manual input events
            try
            {
                InputMonitor.Instance.Register(this as IInputCommandListener, InputProviderType.KeyboardAll);
            }
            catch (System.NullReferenceException) { }
        }

        public void InputArrived(object sender, InputCommandArgs args)
        {
            Debug.Log(">> Target >> InputArrived! = " + args.Action);
            switch (args.Action)
            {
                case InputCommand.SelectionKeyPressed:
                    if (HitTarget != null)
                    {
                        HitTarget.SendMessage("OnSelect",SendMessageOptions.DontRequireReceiver);
                    }
                    break;
            }
        }
        #endregion
    }
}
