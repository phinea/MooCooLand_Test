using UnityEngine;
using System.Collections;

public class DoNotRender : MonoBehaviour {

	// Use this for initialization
	void Start () {
        this.GetComponent<Renderer>().enabled = false;
	}
}
