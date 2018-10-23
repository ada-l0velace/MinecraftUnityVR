using UnityEngine;
using System.Collections;

public class VRMinotauroCollision : MonoBehaviour
{
	public Transform flames;

	void FixedUpdate()
    {
		if (VRStare_and_Grab.Grab) {
			//GameObject.Find ("Big Flames").SetActive(false);
			Ray ray = Camera.main.ScreenPointToRay (transform.position);
			RaycastHit hit;

			if (Physics.Raycast (ray, out hit, 30.0f)) {
				Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.forward) * 30, Color.red); 
				//Debug.DrawLine (ray.origin, hit.point, Color.green);
				//Debug.Log (hit.transform.tag);
				if (hit.transform.tag.Equals ("minotauro")) {
					//GameObject.Find ("Big Flames").SetActive(true);
					flames.gameObject.SetActive (true);
					Destroy (hit.transform.gameObject);
				}
			} else {
				Debug.DrawRay (transform.position, transform.TransformDirection (Vector3.forward) * 30, Color.white); 
				//Debug.DrawLine (vrCamera.position,vrCamera.TransformDirection(Vector3.forward), Color.white); 
				//Debug.DrawLine (transform.position,transform.parent.TransformDirection(a), Color.green); 

			}
		}
    }
}