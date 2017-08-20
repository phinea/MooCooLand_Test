using MooCooEngine.Game;
using UnityEngine;

public enum TargetType
{
    any,
    none,
    one,
    two,
    three
}

namespace MooCooEngine
{
    public class Target_WithScore : Target
    {
        public GameObject explosion;

        public TargetType TargetCategory = TargetType.none;
        public int ScoreOnHit = 0;

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
            GameObject.DestroyObject(this);
        }
    }
}