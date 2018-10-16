using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Minimap : MonoBehaviour {

	public Transform player;
	public Transform vrCamera;
	public Transform arrow;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	// Its called after Update and fixed update so we only update our minimap after the player has moved.
	void LateUpdate() {
		Vector3 newPosition = vrCamera.position;
		newPosition.y = transform.position.y;
		transform.position = newPosition;
		transform.rotation = Quaternion.Euler (90f, vrCamera.eulerAngles.y, 0f);
	
	}
}
