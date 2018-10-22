using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VRStare_and_Grab : MonoBehaviour {

    public float stare_time = 0f; // timer 
    public bool Grab = false;
    public Transform VRHand;
    public Rigidbody TargetObject;
    public Vector3 HandRotation;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
        stare_time = stare_time + Time.deltaTime;

        if (stare_time >= 3f) // once a certain amount of time has passed, the object will be 'grabbed'
        {
            Grab = true;
        }

        if (Grab) // Object won't be dropped even if it isn't looked at
        {
            GrabObject();
        }
    }

    public void ResetStareTime()
    {
        stare_time = 0f;
    }

    public void GrabObject() // Object will be maintained where the VRHand is and will mimick rotation
    {
        TargetObject.transform.position = Vector3.Lerp(TargetObject.transform.position, VRHand.transform.position, 1);
        TargetObject.transform.rotation = Quaternion.Euler(new Vector3(VRHand.eulerAngles.x, VRHand.eulerAngles.y, VRHand.eulerAngles.z));

        TargetObject.transform.parent = VRHand.transform;
    }
}
