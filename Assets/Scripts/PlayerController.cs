using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public Transform vrCamera;
	public float toggleAngle = 45.0f;
	public float speed = 0.5f;
	public bool moveForward;
	private CharacterController cc;
	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		moveForward = (vrCamera.eulerAngles.x >= toggleAngle && vrCamera.eulerAngles.x < 90.0f) ? true : false;
		if (Input.GetKeyDown ("f"))
			moveForward = !moveForward;
		if (moveForward && cc.isGrounded) {
			Vector3 forward = vrCamera.TransformDirection (Vector3.forward);
			cc.Move (speed * forward);
		} else {
            cc.Move(Physics.gravity);
        }
	}
}
