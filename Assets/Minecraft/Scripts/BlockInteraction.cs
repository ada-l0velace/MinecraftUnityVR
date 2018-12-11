using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlockInteraction : MonoBehaviour {

	public Transform vrCamera;
	public GameObject model;
	public GameObject model2;
	private Animator playerAnimator;
	public GameObject arm;
	Vector3 old_pos;
	// Use this for initialization

	void Start () {
		playerAnimator = model.GetComponent<Animator> ();
		World.Instance.character.audio = GetComponent<AudioSource>();
		World.Instance.mob.audio = World.Instance.mob_o.GetComponent<AudioSource> ();
		old_pos = arm.transform.position;
	}

	public IEnumerator lol() {
		yield return new WaitForSeconds (0.2f);
		arm.transform.position = old_pos;
	}

	
	// Update is called once per frame
	void Update () {
		bool pcKeys = Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1);
		bool androidKeys = Input.GetKeyDown ("joystick button 0") || Input.GetKeyDown ("joystick button 2");
		bool keys = androidKeys || pcKeys;
		bool destroyKeys = Input.GetMouseButtonDown (0) || Input.GetKeyDown ("joystick button 0");
		// .SetBool ("attacking", true);
		if (keys) {
			RaycastHit hit;
			//Debug.DrawRay (vrCamera.transform.position, vrCamera.TransformDirection (Vector3.forward)*1);
			if (Physics.Raycast (vrCamera.transform.position, vrCamera.TransformDirection (Vector3.forward), out hit, 5)) {
				Chunk hitc;
				if (hit.collider.gameObject.name == "Zombie") {
					World.Instance.character.attack(World.Instance.mob);
					//old_pos = arm.transform.position;
					//arm.transform.position = hit.point;
					//StartCoroutine ("lol");
					/*renderer.color = Color.red;
					Invoke("ResetColor", flashTime);

					renderer.color = origionalColor;
					*/
					NavMeshAgent nav = World.Instance.mob_o.GetComponent<NavMeshAgent>();
					Vector3 npcPos = World.Instance.mob_o.transform.position;
					Vector3 direction1 = (npcPos - World.Instance.player.transform.position).normalized;
					direction1 = direction1 * 2 / 2;
					direction1 = new Vector3(direction1.x, 0f, direction1.z);
					nav.Move(direction1);
					if (World.Instance.mob.isCharacterDead ())
						DestroyImmediate (World.Instance.mob_o);
					return;
				}
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

				if (destroyKeys) {
					playerAnimator.SetTrigger ("attacking");
					update = hitc.chunkData [x, y, z].HitBlock (hitBlock);
				}
				else {
					//update = b.BuildBlock (new Stone (b.position, b.owner));
					update = b.BuildBlock (BlockFactory.Get (World.Instance.character.inventory.getSelectedItem().getBlockType (), b.position, b.owner));
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
					World.Instance.meshSurface.UpdateNavMesh(World.Instance.meshSurface.navMeshData);
				}
			}

		}
	}
}
