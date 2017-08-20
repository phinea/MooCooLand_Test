using System;
using UnityEngine;
using MooCooEngine;

namespace MooCooEngine
{
    public enum HoverTransitionType
    {
        Boolean,
        LinearBlend,
        SlowInitialIncrease,
        FastInitialIncrease,
    }

    public class HitBehavior_Hover : MonoBehaviour
    {
        // --- PUBLIC ---
        public bool useVisualFeedbackManager = true;

        // Overlay Feedback
        public GameObject _Template;
        public bool UseTextMeshHighlight = false;
        public TextMesh txtMesh;

        public bool _ShowOverlay;
        public int _MinDwellTime = 50; // in ms; Minimal dwell time to trigger any feedback; Used to slowly blend in the feedback
        public int _MaxDwellTime = 2000; // in ms; Max dwell time to trigger full feedback
        public int _MinLookAwayTime = 50; // in ms; Max time to look away until feedback starts to fade
        public int _MaxLookAwayTime = 500; // in ms; Max time to look away until feedback starts to 

        // Highlighting
        public bool _ShowObjHighlight = false;
        public Color _HighlightObjColor = Color.white;

        // --- PRIVATE ---
        private GameObject VisualFeedbackTemplate;
        private HoverTransitionType transType = HoverTransitionType.LinearBlend;
        private bool showOverlay;
        private int minDwellTime = 50; // in ms; Minimal dwell time to trigger any feedback; Used to slowly blend in the feedback
        private int maxDwellTime = 5000; // in ms; Max dwell time to trigger full feedback
        private int minLookAwayTime = 50; // in ms; Max time to look away until feedback starts to fade
        private int maxLookAwayTime = 1000; // in ms; Max time to look away until feedback starts to fade

        private bool showObjHighlight;
        private Color highlightObjColor;
        private Color[] originalColors;

        private GameObject localVisFeedback;
        private bool cursorInsideBounds = false;
        private DateTime t_OnCursorLeave;
        private DateTime t_OnCursorEnter; // aka first looked at

        private float normalizedInterest = 0;
        private float reachedInterest = 0;
        private bool highlightOn = false;

        


        // Use this for initialization
        void Start()
        {
            t_OnCursorEnter = DateTime.MaxValue;

            Target target = (Target)GetComponent<Target>();
            if (target != null)
            {
                target.OnCursorEnter += Target_OnCursorEnter;
                target.OnCursorLeave += Target_OnCursorLeave;
                target.OnTargetSelected += Target_OnTargetSelected;

                target.OnCursorMove += Target_OnCursorMove;
            }

            LoadDataFromManager();
            
            if ((UseTextMeshHighlight)&&(txtMesh == null))
            {
                txtMesh = this.gameObject.transform.parent.GetComponentInChildren<TextMesh>();
            }

            GetOriginalColor();
        }

        public void LoadDataFromManager()
        {
            // Look if there is a global manager for the visual feedback
            if (useVisualFeedbackManager)
            {
                VisualFeedbackManager vfm = FindObjectOfType<VisualFeedbackManager>();

                if (vfm != null)
                {
                    VisualFeedbackTemplate = vfm.Template;
                    minDwellTime = vfm.minDwellTime;
                    maxDwellTime = vfm.maxDwellTime;
                    minLookAwayTime = vfm.minLookAwayTime;
                    maxLookAwayTime = vfm.maxLookAwayTime;
                    showOverlay = vfm.showOverlay;
                    showObjHighlight = vfm.highlightObj;
                    highlightObjColor = vfm.highlightObjColor;
                    transType = vfm.transitionType;
                    return;
                }
            }

            VisualFeedbackTemplate = _Template;
            minDwellTime = _MinDwellTime;
            maxDwellTime = _MaxDwellTime;
            minLookAwayTime = _MinLookAwayTime;
            maxLookAwayTime = _MaxLookAwayTime;
            showOverlay = _ShowOverlay;
            showObjHighlight = _ShowObjHighlight;
            highlightObjColor = _HighlightObjColor;
        }

        void Update()
        {
            if ((cursorInsideBounds) || (normalizedInterest > 0))
            {
                // Update visual feedback
                LoadDataFromManager();
                
                // Handle target confidence
                if (cursorInsideBounds) //TODO: Not just inside bounds, but rather... is it in a cluster?
                    IncreaseInterest();
                else
                    DecreaseInterest();

                //// Show feedback
                ShowFeedback(normalizedInterest);
                UpdateLocalOverlayPosition();
            }

            if ((!cursorInsideBounds) && (normalizedInterest == 0))
            {
                DestroyLocalFeedback();
            }
        }

        private float NormalizedInterest_Dwell()
        {
            // Determine normalized "interest" based on dwell time
            //Debug.Log(">>normalizedInterest = " + normalizedInterest);
            return ClampNormalizedValue((float)((DwellTime - minDwellTime) / (maxDwellTime - minDwellTime)));
        }

        private float NormalizedInterest_LookAway()
        {
            // Determine normalized "decreased interest" based on look away time
            return ClampNormalizedValue((float)((LookAwayTime - minLookAwayTime) / (maxLookAwayTime - minLookAwayTime)));
        }

        private void IncreaseInterest()
        {
            if (normalizedInterest == 0)
                GetOriginalColor();

            normalizedInterest = (float)NormalizedInterest_Dwell();
            reachedInterest = normalizedInterest;
        }

        private void DecreaseInterest()
        {
            normalizedInterest = ClampNormalizedValue(reachedInterest - (float)NormalizedInterest_LookAway());
        }

        private float ClampNormalizedValue(float value)
        {
            if (value > 1)
                value = 1;

            if (value < 0)
                value = 0;

            return value;
        }

        private void DestroyLocalFeedback()
        {
            if (localVisFeedback != null)
                Destroy(localVisFeedback, 0);
            highlightOn = false;

            // ResetObjectHighlighting
        }

        /// <summary>
        /// Shows different types of visual feedback based on given normalized interest level (0-no interest; 1-full interest). 
        /// </summary>
        /// <param name="normalizedInterest"></param>
        private void ShowFeedback(float normalizedInterest)
        {
            if (highlightOn)
            {
                // Try out different transitions
                normalizedInterest = TransitionAdjustedInterest(normalizedInterest);
                //# HIGHLIGHTING with a given material
                if (showOverlay)
                {
                    try
                    {
                        //Renderer[] renderers = localVisFeedback.GetComponents<Renderer>();
                        //foreach (Renderer r in renderers)
                        //{
                        //    //r.material.color = Color.white; //highlightOn ? this.HighlightColor : this.NormalColor;
                        //    Color c = r.material.color;
                        //    //c.a = normalizedInterest;
                        //    //r.material.color = c;
                        // }

                        if ((normalizedInterest > 0) && (normalizedInterest <= 1) && (localVisFeedback != null))
                            localVisFeedback.SetActive(true);
                    }
                    catch (MissingReferenceException)
                    {
                        // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
                    }
                }

                //# HIGHLIGHTING the object itself
                if (showObjHighlight)
                    ShowObjHighlights(normalizedInterest);                 
            }
            else
            {
                normalizedInterest = 0;
                if (showObjHighlight)
                    ShowObjHighlights(0);

                if(localVisFeedback != null)
                    localVisFeedback.SetActive(false);
            }
        }

        private void ChangeRenderColor() { }
        private void ChangeTextColor() { }

        private void ShowObjHighlights(float normInterest)
        {
            if (showObjHighlight)
            {
                if (txtMesh != null)
                {
                    txtMesh.color = BlendColors(originalColors[0], highlightObjColor, normInterest);
                    if(normInterest == 0)
                        txtMesh.fontSize = 16;
                    else
                        txtMesh.fontSize = 24;
                }
                else
                {
                    try
                    {
                        Renderer[] renderers = this.GetComponents<Renderer>();
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            Material[] mats = renderers[i].materials;
                            foreach (Material mat in mats)
                            {
                                mat.color = BlendColors(originalColors[i], highlightObjColor, normInterest);
                            }
                        }

                        renderers = this.GetComponentsInChildren<Renderer>();
                        for (int i = 0; i < renderers.Length; i++)
                        {
                            Material[] mats = renderers[i].materials;
                            foreach (Material mat in mats)
                            {
                                mat.color = BlendColors(originalColors[i], highlightObjColor, normInterest);
                            }
                        }
                    }
                    catch (MissingReferenceException)
                    {
                        // Just ignore; Usually happens after the game object already got destroyed, but the update sequence had already be started
                    }
                }
            }
        }


        private float TransitionAdjustedInterest(float normalizedInterest)
        {
            float quadIncreasePower = 4;
            float logIncreasePower = 3;

            switch (transType)
            {
                case HoverTransitionType.Boolean:
                    if (normalizedInterest > 0)
                        return 1;
                    else return 0;
                case HoverTransitionType.LinearBlend:
                    return normalizedInterest;
                case HoverTransitionType.SlowInitialIncrease:
                    return (float)Math.Pow(normalizedInterest, quadIncreasePower);
                case HoverTransitionType.FastInitialIncrease:
                    if (normalizedInterest == 0)
                        return 0;
                    else
                    {
                        double min = Math.Log(0.00001);
                        double max = Math.Log(1);
                        float val = (float)((Math.Log(normalizedInterest) - min) / (max - min));
                        return (float)Math.Pow(val, logIncreasePower);
                    }
                default:
                    return normalizedInterest;
            }
        }


        private Color BlendColors(Color origColor, Color highlightColor, float normalizedBlendFactor)
        {
            Color c = origColor;
            c.r = divBlendColor(origColor.r, highlightColor.r, normalizedBlendFactor);
            c.g = divBlendColor(origColor.g, highlightColor.g, normalizedBlendFactor);
            c.b = divBlendColor(origColor.b, highlightColor.b, normalizedBlendFactor);
            c.a = divBlendColor(origColor.a, highlightColor.a, normalizedBlendFactor);
            return c;
        }

        private float divBlendColor(float orig, float highl, float normalizedBlendFactor)
        {
            return (orig + (highl - orig) * normalizedBlendFactor);
        }


        private void Target_OnCursorLeave(object sender, TargetEventArgs e)
        {
            cursorInsideBounds = false;
            t_OnCursorLeave = DateTime.Now;
        }

        private void Target_OnCursorEnter(object sender, TargetEventArgs e)
        {
            // Local copy
            double howLongLookedAway = LookAwayTime;

            // Set variables to show feedback
            cursorInsideBounds = true;


            // Reset dwell timer if necessary
            if ((howLongLookedAway > maxLookAwayTime) || (t_OnCursorEnter == DateTime.MaxValue))
            {
                t_OnCursorEnter = DateTime.Now;
            }

            if (localVisFeedback == null)
            {
                normalizedInterest = 0;
                reachedInterest = 0;

                //SphereCollider sc = GetComponent<SphereCollider>();
                //BoxCollider bc = GetComponent<BoxCollider>();

                //Vector3 pos;
                //if (bc != null)
                //    pos = transform.position + bc.center / 10;
                //else if (sc != null)
                //    pos = transform.position + sc.center / 10;
                //else
                //    pos = transform.position;

                if (VisualFeedbackTemplate != null)
                {
                    localVisFeedback = (GameObject)Instantiate(VisualFeedbackTemplate, new Vector3(0, 0, 0), VisualFeedbackTemplate.transform.rotation);

                    UpdateLocalOverlayPosition();
                    localVisFeedback.SetActive(false);
                    
                }
                ShowFeedback(normalizedInterest);
            }

            prevGazePos = e.Cursor.ScreenPos;
        }

        /// <summary>
        /// In case the currently hovered target is moving, let's also update the overlay feedback.
        /// </summary>
        private void UpdateLocalOverlayPosition()
        {
            if (showOverlay && (localVisFeedback != null) && (localVisFeedback.activeSelf))
            {
                SphereCollider sc = GetComponent<SphereCollider>();
                BoxCollider bc = GetComponent<BoxCollider>();

                if (bc != null)
                    localVisFeedback.transform.position = transform.position + bc.center / 10;
                else if (sc != null)
                    localVisFeedback.transform.position = transform.position + sc.center / 10;
                else
                    localVisFeedback.transform.position = transform.position;
            }
        }

        Vector2 prevGazePos;
        private void Target_OnCursorMove(object sender, TargetEventArgs e)
        {
            if (!highlightOn)
            {
                float speed = GazeGradient(prevGazePos, e.Cursor.ScreenPos);

                //GazeAnalyzer gazeAnalyzer = FindObjectOfType<GazeAnalyzer>();
                //if (((gazeAnalyzer == null) && (speed < 10)) || ((gazeAnalyzer != null) && (gazeAnalyzer.IsDwelling())))
                {
                    highlightOn = true;
                }
            }
            prevGazePos = e.Cursor.ScreenPos;
        }

        private void Target_OnTargetSelected(object sender, TargetEventArgs e)
        {
            if (localVisFeedback != null)
                Destroy(localVisFeedback, 0);
        }

        /// <summary>
        /// Amount of time the user looked away from this target in milliseconds
        /// </summary>
        private double LookAwayTime
        {
            get
            {
                if (cursorInsideBounds)
                    return 0;
                else
                    return (DateTime.Now - t_OnCursorLeave).TotalMilliseconds;
            }
        }

        private double DwellTime
        {
            get
            {
                if (!cursorInsideBounds)
                    return 0;
                else
                    return (DateTime.Now - t_OnCursorEnter).TotalMilliseconds;
            }
        }

        float lmax = float.MinValue;
        float lmin = float.MaxValue;

        private float GazeGradient(Vector2 pos1, Vector2 pos2)
        {
            Vector2 g = pos1 - pos2;
            return g.magnitude;
        }

        void GetOriginalColor()
        {
            if (txtMesh != null)
            {
                originalColors = new Color[1];
                originalColors[0] = txtMesh.color;
            }
            else
            {
                Renderer[] renderers = GetComponentsInChildren<Renderer>();
                originalColors = new Color[renderers.Length];

                for (int i = 0; i < renderers.Length; i++)
                {
                    originalColors[i] = renderers[i].material.color;
                }
            }
        }
    }
}