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
		bool bButtonDown_app = GvrControllerInput.AppButton;
		bool bButtonDown_touch = GvrControllerInput.ClickButton;
		bool bTouchingPad = GvrControllerInput.IsTouching;
		Vector2 vTouchPos = GvrControllerInput.TouchPos;
	}
	
	// Update is called once per frame
	void Update () {
		moveForward = GvrControllerInput.IsTouching;
		moveBackward = false;
		//Debug.Log (vrCamera.eulerAngles.x + moveBackward.ToString() + moveForward.ToString());
		if (moveForward && cc.isGrounded) {
			Vector3 forward = vrCamera.TransformDirection (Vector3.forward );
			forward.y = 0;
			cc.Move (speed * forward * Time.deltaTime);
		} else if (moveBackward && cc.isGrounded) {
			Vector3 backward = vrCamera.TransformDirection (Vector3.back);
			backward.y = 0;
			//cc.Move (speed * backward * Time.deltaTime);
		}
		else if (GvrControllerInput.AppButtonUp && cc.isGrounded){
			Vector3 upward = vrCamera.TransformDirection (Vector3.up);
			cc.Move (speed * 30* upward * Time.deltaTime);
		}
		else if (GvrControllerInput.ClickButtonUp) {
			RaycastHit hit;
			if (Physics.Raycast (vrCamera.transform.position, vrCamera.TransformDirection (Vector3.forward), out hit, 10)) {
				Vector3 hitBlock = hit.point - hit.normal / 2.0f;

				int x = (int)(Mathf.Round (hitBlock.x) - hit.collider.gameObject.transform.position.x);
				int y = (int)(Mathf.Round (hitBlock.y) - hit.collider.gameObject.transform.position.y);
				int z = (int)(Mathf.Round (hitBlock.z) - hit.collider.gameObject.transform.position.z);

				List<string> updates = new List<string> ();
				float thisChunkx = hit.collider.gameObject.transform.position.x;
				float thisChunky = hit.collider.gameObject.transform.position.y;
				float thisChunkz = hit.collider.gameObject.transform.position.z;

				updates.Add (hit.collider.gameObject.name);
				if (x == 0)
					updates.Add (World.BuildChunkName (new Vector3(thisChunkx-World.chunkSize, thisChunky, thisChunkz)));
				if (x == World.chunkSize-1)
					updates.Add (World.BuildChunkName (new Vector3(thisChunkx+World.chunkSize, thisChunky, thisChunkz)));

				if (y == 0)
					updates.Add (World.BuildChunkName (new Vector3(thisChunkx, thisChunky-World.chunkSize, thisChunkz)));
				if (y == World.chunkSize-1)
					updates.Add (World.BuildChunkName (new Vector3(thisChunkx, thisChunky+World.chunkSize, thisChunkz)));

				if (z == 0)
					updates.Add (World.BuildChunkName (new Vector3(thisChunkx, thisChunky, thisChunkz-World.chunkSize)));
				if (z == World.chunkSize-1)
					updates.Add (World.BuildChunkName (new Vector3(thisChunkx, thisChunky, thisChunkz+World.chunkSize)));
				

				foreach (string cname in updates) {
					Chunk c;
					if (World.chunks.TryGetValue (cname, out c)) {
						DestroyImmediate (c.chunk.GetComponent<MeshFilter> ());
						DestroyImmediate (c.chunk.GetComponent<MeshRenderer> ());
						DestroyImmediate (c.chunk.GetComponent<Collider> ());
						c.chunkData [x, y, z].SetType (Block.BlockType.AIR);
						c.DrawChunk ();
					}
				}
			}
		}
		else if (!cc.isGrounded) {
			cc.Move(Physics.gravity * Time.deltaTime);
        }

	}
}
