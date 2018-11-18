using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OcclusionObject : MonoBehaviour {

	Renderer myRend;
	public float displayTime;
	// Use this for initialization
	void Start () {
		myRend = gameObject.GetComponent<Renderer> ();
		displayTime = -1;
	}
	
	// Update is called once per frame
	void Update () {
		if (myRend) {
			if (displayTime > 0) {
				displayTime -= Time.deltaTime;
				//turnOffRender (true);
				myRend.enabled = true;
			}
			else {
				//turnOffRender (false);
				myRend.enabled = false;
			}
		}
	}

	void turnOffRender(bool lol) {
		//Debug.Log (gameObject.transform.parent);
		Transform[] ts = gameObject.transform.parent.GetComponentsInChildren<Transform> ();
		foreach (Transform t in ts) {
			if (t.gameObject.GetComponent<Renderer> () != null)
				t.gameObject.GetComponent<Renderer> ().enabled = lol;
			//Debug.Log (t);
			//t.gameObject.GetComponent<Renderer>().enabled = lol;
		}
	}

	public void HitOcclude(float time) {
		displayTime = time;
		myRend.enabled = true;
	}
}
