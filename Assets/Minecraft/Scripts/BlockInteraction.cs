using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockInteraction : MonoBehaviour {

	public Transform vrCamera;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		bool pcKeys = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
		bool androidKeys = Input.GetKeyDown ("joystick button 0") || Input.GetKeyDown ("joystick button 2");
		bool keys = androidKeys || pcKeys;
		bool destroyKeys = Input.GetMouseButtonDown (0) || Input.GetKeyDown ("joystick button 0");
		if (keys) {
			RaycastHit hit;
			//Debug.DrawRay (vrCamera.transform.position, vrCamera.TransformDirection (Vector3.forward)*1);
			if (Physics.Raycast (vrCamera.transform.position, vrCamera.TransformDirection (Vector3.forward), out hit, 10)) {
				Chunk hitc;
				if(!World.chunks.TryGetValue (hit.collider.gameObject.name, out hitc)) return;
				
				Vector3 hitBlock;
				if(destroyKeys)
					hitBlock = hit.point - hit.normal / 2.0f;
				else
					hitBlock = hit.point + hit.normal / 2.0f;

				bool update = false;

				Block b = World.GetWorldBlock (hitBlock);
				int x = (int)b.position.x;
				int y = (int)b.position.y;
				int z = (int)b.position.z;

				if (destroyKeys)
					update = hitc.chunkData [x, y, z].HitBlock ();
				else {
					b.owner.chunkData [x, y, z] = new Stone (b.position, b.owner);
					b.owner.ReDraw ();
					update = true;
				}

				if (update) {
					List<string> updates = new List<string> ();

					float thisChunkx = hitc.chunkData[x,y,z].position.x;
					float thisChunky = hitc.chunkData[x,y,z].position.y;
					float thisChunkz = hitc.chunkData[x,y,z].position.z;

					if (x == 0)
						updates.Add (World.BuildChunkName (new Vector3 (thisChunkx - World.chunkSize, thisChunky, thisChunkz)));
					if (x == World.chunkSize - 1)
						updates.Add (World.BuildChunkName (new Vector3 (thisChunkx + World.chunkSize, thisChunky, thisChunkz)));

					if (y == 0)
						updates.Add (World.BuildChunkName (new Vector3 (thisChunkx, thisChunky - World.chunkSize, thisChunkz)));
					if (y == World.chunkSize - 1)
						updates.Add (World.BuildChunkName (new Vector3 (thisChunkx, thisChunky + World.chunkSize, thisChunkz)));

					if (z == 0)
						updates.Add (World.BuildChunkName (new Vector3 (thisChunkx, thisChunky, thisChunkz - World.chunkSize)));
					if (z == World.chunkSize - 1)
						updates.Add (World.BuildChunkName (new Vector3 (thisChunkx, thisChunky, thisChunkz + World.chunkSize)));

					foreach (string cname in updates) {
						Chunk c;
						if (World.chunks.TryGetValue (cname, out c)) {
							//Debug.Log (x + " " + y + " " + z + " "+ hit.collider.gameObject.name + " "+ hitBlock.ToString() + " "+  hit.collider.gameObject.transform.position.ToString());
							//Debug.Log (c.chunkData[x,y,z].bType.ToString());
							c.ReDraw ();
						}
					}
				}

			}
		}
	}
}
