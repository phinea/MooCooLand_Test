using UnityEngine;

namespace MooCooEngine
{
    public class KeepThisAlive : Singleton<KeepThisAlive>
    {
        // Use this for initialization
        void Start()
        {
            //# If instance does not exist yet, let's create one and tell the system to keep it alive across scenes.
            if (KeepThisAlive.Instance == null)
            {
                KeepThisAlive.Instance = this;
                DontDestroyOnLoad(KeepThisAlive.Instance);
            }
            //# If an instance already exists, let's deactivate this local instance. 
            else // if (KeepThisAlive.Instance != null)
            {
                this.gameObject.SetActive(false);
                Instance.gameObject.SetActive(true);
            }
           
        }
        
        void OnDestroy()
        {
            Debug.Log(">> KeepThisAlive >> Destroy it! ");
        }
    }
}
