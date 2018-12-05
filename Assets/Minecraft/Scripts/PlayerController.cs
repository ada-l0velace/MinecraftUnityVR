using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	public Transform vrCamera;
	public Transform player;
	public Vector3 initial;
	// Use this for initialization
	void Start () {
		initial = transform.position;
	}

	// Update is called once per frame
	void Update () {

	}

	// Its called after Update and fixed update so we only update our minimap after the player has moved.
	void LateUpdate() {
		
		//Vector3 backward = vrCamera.TransformDirection (Vector3.back);
		//Debug.Log (backward);
		//if (Input.GetKeyDown("space")) {
		//Vector3 cameraDir = vrCamera.TransformDirection (Vector3.back)*2.5f;
		//Debug.Log (vrCamera.eulerAngles);
		//transform.position = player.transform.position.x + cameraDir.x;

		//transform.position = new Vector3(player.transform.position.x + cameraDir.x, transform.position.y, player.transform.position.z + cameraDir.z) ;
		//}
		transform.rotation = Quaternion.Euler (0f, vrCamera.eulerAngles.y, 0f);

	}
}