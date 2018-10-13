using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public Transform vrCamera;
	public float forwardAngle = 30.0f;
	public float backwardAngle = 330.0f;
	public float speed = 0.5f;
	public bool moveForward;
	public bool moveBackward;
	private CharacterController cc;
	// Use this for initialization
	void Start () {
		cc = GetComponent<CharacterController>();
	}
	
	// Update is called once per frame
	void Update () {
		moveForward = (vrCamera.eulerAngles.x >= forwardAngle && vrCamera.eulerAngles.x < 90.0f) ? true : false;
		moveBackward = (vrCamera.eulerAngles.x <= backwardAngle && vrCamera.eulerAngles.x > 180.0f) ? true : false;
		Debug.Log (vrCamera.eulerAngles.x + moveBackward.ToString() + moveForward.ToString());
		if (moveForward && cc.isGrounded) {
			Vector3 forward = vrCamera.TransformDirection (Vector3.forward);
			forward.y = 0;
			cc.Move (speed * forward);
		} else if (moveBackward && cc.isGrounded) {
			Vector3 backward = vrCamera.TransformDirection (Vector3.back);
			backward.y = 0;
			cc.Move (speed * backward);
		}
		else {
            cc.Move(Physics.gravity);
        }
	}
}
