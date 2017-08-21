using MooCooEngine.Game;
using UnityEngine;

namespace MooCooEngine
{
    public class Target_ShootingGallery : Target
    {
        public GameObject explosion;
        public int ScoreOnHit = 0;

        [Tooltip ("Probability for this object to appear in the Shooting Gallery.")]
        [Range (0,100)]
        public int ProbabilityInPercent = 0;

        internal Vector3 MovingDir;

        public new void OnSelect()
        {
            //# Add score
            if (LevelManager.Instance != null)
            {
                LevelManager.Instance.Score_Increment(ScoreOnHit);
            }

            //# Show animation for selection
            gameObject.SetActive(false);

            if (explosion != null)
            {
                Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
            }

            //# Destroy selected object
            ShootingGalleryManager.Instance.RemoveTarget(this.gameObject);
        }
    }
}