using UnityEngine;

namespace MooCooEngine
{
    public class VisualFeedbackManager : MonoBehaviour
    {
        public bool showOverlay;
        public GameObject Template;
        public int minDwellTime = 0; // in ms; Minimal dwell time to trigger any feedback; Used to slowly blend in the feedback
        public int maxDwellTime = 1500; // in ms; Max dwell time to trigger full feedback
        public int minLookAwayTime = 50; // in ms; Max time to look away until feedback starts to fade
        public int maxLookAwayTime = 500; // in ms; Max time to look away until feedback starts to fade
        public bool highlightObj;
        public Color highlightObjColor;
        public HoverTransitionType transitionType = HoverTransitionType.Boolean;
    }
}