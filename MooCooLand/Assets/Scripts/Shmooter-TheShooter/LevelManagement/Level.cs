using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MooCooEngine.Game
{
    /// <summary>
    /// Generic description of the current level. Later will be loaded from an XML file.
    /// </summary>
    public class Level : MonoBehaviour
    {
        public string LevelName;
        public string LevelID;

        // TODO: Add other descriptions... particular layouts, etc.

        public bool isTimed = true;
        public float timerMaxInSec = 100;

        
    }

    public enum SupportedTargetTypes
    {
        Target1,
        Target2,
    }

    public enum SupportedDistractorTypes
    {
        DistractorType1,
        DistractorType2,
    }
}
