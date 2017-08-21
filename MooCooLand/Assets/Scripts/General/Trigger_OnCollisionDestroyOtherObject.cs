using MooCooEngine.Game;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_OnCollisionDestroyOtherObject : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        ShootingGalleryManager.Instance.RemoveTarget(collision.gameObject);
    }
}
