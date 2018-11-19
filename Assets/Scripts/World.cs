using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class World : MonoBehaviour {

	public GameObject player;
	public Material textureAtlas;
	public static int columnHeight = 16;
	public static int chunkSize = 16;
	public static int worldSize = 2;
	public static int radius = 1;
	public static Dictionary<string, Chunk> chunks;
	public Slider loadingAmount;
	public Camera cam;
	bool firstbuild = true;
	bool building = false;

	public static string BuildChunkName(Vector3 v) {
		return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
	}

	 IEnumerator BuildWorld() {
		building = true;

		int posx = (int)Mathf.Floor (player.transform.position.x / chunkSize);
		int posz = (int)Mathf.Floor (player.transform.position.z / chunkSize);


		float totalChunks = (Mathf.Pow(radius*2,2) * columnHeight) * 2;
		int processCount = 0;

		for (int z = -radius; z < radius; z++) {
			for (int x = -radius; x < radius; x++) {
				for (int y = 0; y < columnHeight; y++) {
					
					Vector3 chunkPosition = new Vector3 ((x+posx) * chunkSize, y * chunkSize, (posz+z) * chunkSize);
					Chunk c; //= new Chunk (chunkPosition, textureAtlas);
					string n = BuildChunkName(chunkPosition);
					if (chunks.TryGetValue (n, out c)) {
						c.status = Chunk.ChunkStatus.KEEP;
						break;
					} else {
						c = new Chunk (chunkPosition, textureAtlas);
						c.chunk.transform.parent = this.transform;
						chunks.Add (c.chunk.name, c);
					}
					if (firstbuild) {
						processCount++;
						loadingAmount.value = processCount / totalChunks;
					}
					yield return null;

				}
			}
		}

		foreach (KeyValuePair<string, Chunk> c in chunks) {
			if (c.Value.status == Chunk.ChunkStatus.DRAW) {
				c.Value.DrawChunk ();
				c.Value.status = Chunk.ChunkStatus.KEEP;
			}

			// delete old chunks

			c.Value.status = Chunk.ChunkStatus.DONE;
			if (firstbuild) {
				processCount ++;
				loadingAmount.value = processCount / totalChunks ;
			}
			yield return null;
		}

		if (firstbuild) {
			cam.gameObject.SetActive (false);
			loadingAmount.gameObject.SetActive (false);
			player.SetActive (true);
			firstbuild = false;
		}
		building = false;

	}

	public void StartBuild() {
		StartCoroutine (BuildWorld ());
	}

	// Use this for initialization
	void Start () {
		player.SetActive (false);
		chunks = new Dictionary<string, Chunk> ();
		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;
		StartCoroutine (BuildWorld ());

	}
	
	// Update is called once per frame
	void Update () {
		if (!building && !firstbuild) {
			StartCoroutine (BuildWorld ());
		}
	}
}
