using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAutomatic : MonoBehaviour {
    public float speed_InDegreePerSecond;
    public Vector3 rotationAxis =  new Vector3(0,1,0);

    public double angle = 0;
    private DateTime tLastUpdate;
    
    // Use this for initialization
    void Start () {
        tLastUpdate = DateTime.UtcNow;
        angle = 0;
	}
	
	// Update is called once per frame
	void Update ()
    {
        angle = (speed_InDegreePerSecond * (DateTime.UtcNow - tLastUpdate).TotalSeconds);
        // angle %= 360;
       // Debug.LogFormat(">> Angle = {0} -- {1} -- {2}", angle, (DateTime.UtcNow - tLastUpdate).TotalSeconds, (DateTime.UtcNow - tLastUpdate).TotalMilliseconds);

        gameObject.transform.Rotate(rotationAxis, (float)angle);

        tLastUpdate = DateTime.UtcNow;
    }
}


