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

        public ShootingGalleryManager AssociatedGameManagerForThisLevel;

        // TODO: Add other descriptions... particular layouts, etc.

        public bool isTimed = true;
        public float timerMaxInSec = 100;

        public void StartLevel()
        {
            if (AssociatedGameManagerForThisLevel != null)
            {
                AssociatedGameManagerForThisLevel.enabled = true;
                AssociatedGameManagerForThisLevel.gameObject.SetActive(true);
            }
        }

        public void FinishLevel()
        {
            if (AssociatedGameManagerForThisLevel != null)
            {
                AssociatedGameManagerForThisLevel.enabled = false;
                AssociatedGameManagerForThisLevel.gameObject.SetActive(false);
                AssociatedGameManagerForThisLevel.RemoveAllTargets();
            }
        }
    }
}
