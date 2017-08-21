using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MooCooEngine.Game
{
    public class ShootingGalleryManager : MonoBehaviour
    {       
        public Target_ShootingGallery[] TargetTemplates; 
        public SpawnTrigger[] TriggersToSpawn; 

        public List<Target_ShootingGallery> spawnedObjList;

        public float spawnFrequencyInSeconds = 1.5f; 
        public float movingSpeed = 0.01f;

        private DateTime tlastUpdated;
        private int counter = 1;

        private static ShootingGalleryManager _instance;
        public static ShootingGalleryManager Instance { get { return _instance; } }

        // Use this for initialization
        void Start()
        {
            if (_instance == null)
                _instance = this;
 
            tlastUpdated = DateTime.MinValue;

            foreach (Target_ShootingGallery tsg in TargetTemplates)
            {
                tsg.gameObject.SetActive(false);
            } 
        }

        public void RemoveTarget(GameObject gobj)
        {
            for (int i = 0; i < spawnedObjList.Count; i++)
            {
                Target_ShootingGallery tsg = spawnedObjList[i];
                if (tsg.name == gobj.name)
                {
                    spawnedObjList.Remove(tsg);
                    Destroy(gobj);
                    return;
                }
            }
        }

        public void RemoveAllTargets()
        {
            Target_ShootingGallery[] list = spawnedObjList.ToArray();
            for (int i = 0; i < list.Length; i++)
            {
                Destroy(list[i].gameObject);
            }
            spawnedObjList = new List<Target_ShootingGallery>();
        }

        // Update is called once per frame
        void Update()
        {
            //# Keep the already spawned items moving
            foreach (Target_ShootingGallery tsg in spawnedObjList)
            {
                try
                {
                    if (tsg != null)
                    {
                        tsg.transform.position += (tsg.MovingDir * movingSpeed) ;
                    }
                    else
                        spawnedObjList.Remove(tsg);
                }
                catch (InvalidOperationException)
                {
                    // This may happen when an object got destroyed externally.
                }
                //tsg.transform.position = new Vector3(0, 0, 2);
            }

            //# Check whether to spawn a new item
            float deltaT = (float)(DateTime.UtcNow - tlastUpdated).TotalSeconds;
            if (deltaT > spawnFrequencyInSeconds)
            {
                // TODO: Pick a "random" object from the templates based on the probability
                if (TargetTemplates.Length > 0)
                {
                    foreach (SpawnTrigger trigger in TriggersToSpawn)
                    {
                        Target_ShootingGallery spawnNewObj = (Target_ShootingGallery)Instantiate(
                            GetSemiRandomTarget(),
                            trigger.transform.position, trigger.transform.rotation);
                        spawnNewObj.gameObject.SetActive(true);
                        spawnNewObj.MovingDir = trigger.MovingDirAfterSpawn;
                        spawnNewObj.name += ("" + counter);
                        counter++;
                        spawnedObjList.Add(spawnNewObj);
                    }
                }
                tlastUpdated = DateTime.UtcNow;
            }
        }

        private float previousRandomIndex = -1;
        private Target_ShootingGallery GetSemiRandomTarget()
        {
            List<Target_ShootingGallery> list = new List<Target_ShootingGallery>();
            foreach (Target_ShootingGallery tsg in TargetTemplates)
            {
                for (int i = 0; i < tsg.ProbabilityInPercent; i++)
                {
                    list.Add(tsg);
                } 
            }

            System.Random rndm = new System.Random();
            UnityEngine.Random r = new UnityEngine.Random();
            int index=  UnityEngine.Random.Range(0, list.Count);
            
            //rndm.Next(0, list.Count);
            //int index = rndm.Next(0, list.Count);
            return (list[index]);
        }
    }
}
