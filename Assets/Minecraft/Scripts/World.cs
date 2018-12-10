using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEditor;
using Realtime.Messaging.Internal;

public class World : MonoBehaviour {

	public GameObject player;
	public Material textureAtlas;
	public Material fluidTexture;

	public static uint maxCoroutines = 5000;
	public static int chunkSize = 8;
	public static int radius = 6;

	public int wtf = 0;
	public static Vector3[] allNormals = new Vector3[6];
	public static Vector3[,,] allVertices = new Vector3[chunkSize+1,chunkSize+1,chunkSize+1];
	public enum NDIR {UP, DOWN, LEFT, RIGHT, FRONT, BACK}

	public CoroutineQueue queue;
	public static ConcurrentDictionary<string, Chunk> chunks;
	public static List<string> toRemove = new List<string> ();

	public bool firstbuild = true;
	public Vector3 lastBuildPos;
	public NavMeshSurface meshSurface;

	public GameObject loadingCam;
	public Slider loadingAmount;
	public static World Instance { get; private set; }

	public static float totalChunks = 231;//(Mathf.Pow(radius,2)) * 2 * 2 -2;
	public int processCount = 0;
	float startTime;
	public static string BuildChunkName(Vector3 v) {
		return (int)v.x + "_" + (int)v.y + "_" + (int)v.z;
	}

	public Character character;

	void Awake() {
		Instance = this;
		character = new Character();
		Debug.Log (totalChunks);
	}
	void BuildChunkAt(int x, int y, int z) {
		Vector3 chunkPostion = new Vector3(x*chunkSize,y*chunkSize,z*chunkSize);

		string n = BuildChunkName (chunkPostion);
		Chunk c;

		if (!chunks.TryGetValue (n, out c)) {
			c = new Chunk (chunkPostion, textureAtlas, fluidTexture);
			c.chunk.transform.parent = this.transform;
			c.fluid.transform.parent = this.transform;
			chunks.TryAdd (c.chunk.name, c);
			//chunks.TryAdd (c.fluid.name, c);
		}
	}

	IEnumerator BuildRecursiveWorld(int x, int y, int z, int rad) {
		
		rad--;
		if(rad <= 0) yield break;

		//build chunk front
		BuildChunkAt(x,y,z+1);
		queue.Run(BuildRecursiveWorld(x,y,z+1,rad));
		yield return null;

		//build chunk back
		BuildChunkAt(x,y,z-1);
		queue.Run(BuildRecursiveWorld(x,y,z-1,rad));
		yield return null;

		//build chunk left
		BuildChunkAt(x-1,y,z);
		queue.Run(BuildRecursiveWorld(x-1,y,z,rad));
		yield return null;

		//build chunk right
		BuildChunkAt(x+1,y,z);
		queue.Run(BuildRecursiveWorld(x+1,y,z,rad));
		yield return null;

		//build chunk up
		BuildChunkAt(x,y+1,z);
		queue.Run(BuildRecursiveWorld(x,y+1,z,rad));
		yield return null;

		//build chunk down
		BuildChunkAt(x,y-1,z);
		queue.Run(BuildRecursiveWorld(x,y-1,z,rad));
		yield return null;

	}

	public static int ConvertBlockIndexToLocal(int i) {
		if(i <= -1) 
			i = World.chunkSize+i; 
		else if(i >= World.chunkSize) 
			i = i-World.chunkSize;
		return i;
	}

	public static Block GetWorldBlock(Vector3 pos) {
		int cx;
		int cy;
		int cz;

		if (pos.x >= 0) {
			cx = (int)(Mathf.Round (pos.x - 0.5f) / (float)chunkSize) * chunkSize;
		} else {
			cx = (int)(Mathf.Round (pos.x + 0.5f- chunkSize) / (float)chunkSize) * chunkSize ;
		}
		if (pos.y >= 0) {
			cy = (int)(Mathf.Round (pos.y - 0.5f) / (float)chunkSize) * chunkSize;
		} else {
			cy = (int)((int)Mathf.Round (pos.y + 0.5f - chunkSize) / (float)chunkSize) * chunkSize ;
		}
		if (pos.z >= 0) {
			cz = (int)(Mathf.Round (pos.z - 0.5f) / (float)chunkSize) * chunkSize;
		} else {
			cz = (int)(Mathf.Round (pos.z + 0.5f- chunkSize) / (float)chunkSize) * chunkSize ;
		}
		int blx = (int) Mathf.Abs((float)Mathf.Round(pos.x-0.5f) - cx);
		int bly = (int) Mathf.Abs((float)Mathf.Round(pos.y-0.5f) - cy);
		int blz = (int) Mathf.Abs((float)Mathf.Round(pos.z-0.5f) - cz);
		//Debug.Log("Block " + blx + " " + bly + " " + blz);

		//blx = ConvertBlockIndexToLocal(blx);
		//bly = ConvertBlockIndexToLocal(bly);
		//blz = ConvertBlockIndexToLocal(blz);


		string cn = BuildChunkName(new Vector3(cx,cy,cz));
		Chunk c;
		//Debug.Log ("Mathf.Round(pos.x) -> " + Mathf.Round (pos.x) + " Mathf.Round(pos.x)/(float)chunkSize -> " + Mathf.Round (pos.x) / (float)chunkSize + " cx ->" + cx);
		//Debug.Log ("Mathf.Round(pos.y) -> " + Mathf.Round (pos.y) + " Mathf.Round(pos.y)/(float)chunkSize -> " + Mathf.Round (pos.x) / (float)chunkSize + " cy ->" + cx);
		//Debug.Log ("Mathf.Round(pos.z) -> " + Mathf.Round (pos.z) + " Mathf.Round(pos.z)/(float)chunkSize -> " + Mathf.Round (pos.x) / (float)chunkSize + " cz ->" + cx);
		//Debug.Log("World Hit: " + pos);
		//Debug.Log("Chunk Hit: " + cn);

		if(chunks.TryGetValue(cn, out c)) {
			//Debug.Log("Block " + blx + " " + bly + " " + blz);
			//Debug.Log (c.chunkData[blx, bly, blz].bType);
			return c.chunkData[blx,bly,blz];
		}
		else
			return null;

	}

	IEnumerator DrawChunks() {
		//toRemove.Clear();
		foreach (KeyValuePair<string, Chunk> c in chunks) {
			if (c.Value.status == Chunk.ChunkStatus.DRAW) {
				c.Value.DrawChunk ();
			}
			if (c.Value.chunk && Vector3.Distance(player.transform.position,
				c.Value.chunk.transform.position) > radius*chunkSize) {
				toRemove.Add (c.Key);
			}
			yield return null;
		}

	}


	IEnumerator RemoveOldChunks() {
		for (int i = 0; i < toRemove.Count; i++) {
			string n = toRemove [i];
			Chunk c;
			//Debug.Log (n);
			if (chunks.TryGetValue (n, out c)) {
				
				Destroy(c.chunk);
				Destroy(c.fluid);
				c.Save ();
				chunks.TryRemove (n, out c);
				yield return null;
			}
		}
		toRemove.Clear ();
	}

	public void BuildNearPlayer() {
		StopCoroutine ("BuildRecursiveWorld");
		queue.Run(BuildRecursiveWorld (
			(int)(player.transform.position.x / chunkSize),
			(int)(player.transform.position.y / chunkSize),
			(int)(player.transform.position.z / chunkSize), radius));
	}

	// Use this for initialization
	void Start () {
		

		//generate all vertices
		for(int z = 0; z <= chunkSize; z++)
			for(int y = 0; y <= chunkSize; y++)
				for(int x = 0; x <= chunkSize; x++) {
					allVertices[x,y,z] = new Vector3(x,y,z);	 
				}
		

		allNormals[(int) NDIR.UP] = Vector3.up;
		allNormals[(int) NDIR.DOWN] = Vector3.down;
		allNormals[(int) NDIR.LEFT] = Vector3.left;
		allNormals[(int) NDIR.RIGHT] = Vector3.right;
		allNormals[(int) NDIR.FRONT] = Vector3.forward;
		allNormals[(int) NDIR.BACK] = Vector3.back;

		Vector3 ppos = player.transform.position;
		player.transform.position = new Vector3 (ppos.x, Utils.GenerateHeight (ppos.x, ppos.z)+5,
			ppos.z);

		lastBuildPos = player.transform.position;
		player.SetActive (false);
			
		firstbuild = true;
		chunks = new ConcurrentDictionary<string, Chunk> ();
		this.transform.position = Vector3.zero;
		this.transform.rotation = Quaternion.identity;
		queue = new CoroutineQueue (maxCoroutines, StartCoroutine);

		//	 starting chunk
		BuildChunkAt ((int)(player.transform.position.x / chunkSize), 
			(int)(player.transform.position.y / chunkSize), 
			(int)(player.transform.position.z / chunkSize));
		startTime = Time.time;
		Debug.Log("Start Build");
		//draw it
		queue.Run(DrawChunks ());

		//create a bigger world
		queue.Run(BuildRecursiveWorld((int)(player.transform.position.x / chunkSize), 
			(int)(player.transform.position.y / chunkSize), 
			(int)(player.transform.position.z / chunkSize), radius));
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 movement = lastBuildPos - player.transform.position;

		if (movement.magnitude > chunkSize) {
			lastBuildPos = player.transform.position;
			BuildNearPlayer ();
		}
		//Debug.Log (queue.numActive);

		if (loadingAmount.value == 1) {
			if (!player.activeSelf) {
				firstbuild = false;
				loadingCam.SetActive (false);
				/*Debug.Log("Built in " + (Time.time - startTime));*/
				player.SetActive (true);
				//Chunk item;
				//chunks.TryGetValue ("8_72_8", out item);

				meshSurface = this.gameObject.AddComponent<NavMeshSurface> ();
				meshSurface.layerMask = 1;
				//StaticEditorFlags.OffMeshLinkGeneration = true;
				meshSurface.BuildNavMesh ();
			
			}
		}
		queue.Run(DrawChunks ());

		//queue.Run(RemoveOldChunks ());
	}
}
