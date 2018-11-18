using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chunks : MonoBehaviour {
	public Transform Stone_block;
	public Transform Dirt_block;
	public Transform Grass_block;

	public GameObject new_block;
	// Use this for initialization
	void Start () {
		GenerateChunk ();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Combine(GameObject block) {
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter> ();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		Destroy (this.gameObject.GetComponent<MeshCollider>());
		Debug.Log (meshFilters.Length);
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters [i].sharedMesh;
			combine[i].transform = meshFilters [i].transform.localToWorldMatrix;
			meshFilters [i].gameObject.SetActive (false);
			i++;
		}

		transform.GetComponent<MeshFilter>().mesh = new Mesh ();
		transform.GetComponent<MeshFilter>().mesh.CombineMeshes (combine, true);
		//transform.GetComponent<MeshFilter>().mesh.RecalculateBounds();
		//transform.GetComponent<MeshFilter>().mesh.RecalculateNormals();


		this.gameObject.AddComponent<MeshCollider>();
		transform.gameObject.SetActive(true);
		Destroy(block);
	}

	void CreateBlock(int y, Vector3 blockPos, bool create) {
		//Debug.Log (blockPos.x + " " + blockPos.y + " " + blockPos.z);
		if (create) {
			Instantiate (Grass_block, blockPos, Quaternion.identity);
			//block.transform.parent = this.transform;
			//Combine (block);
		}
		//else 
		//	Instantiate (Dirt_block, blockPos, Quaternion.identity);
	}

	void GenerateChunk () {
		int depth = 30;
		int width = 30;
		int height = 3;
		int heightScale = 20;
		int heightOffset = 1;
		float detailScale = 25.0f;
		int seed = Random.Range(100000, 999999);
		for (int z = 0; z < depth; z++) {
			for (int x = 0; x < width; x++) {
				int y = (int)(Mathf.PerlinNoise ((x + seed) / detailScale, (z + seed) / detailScale ) * heightScale) * heightOffset;
				//Mathf.Round (transform.position.x, MidpointRounding.AwayFromZero);
				//Mathf.Round (transform.position.y, MidpointRounding.AwayFromZero);
				//Mathf.Round (transform.position.z+1, MidpointRounding.AwayFromZero);
				Vector3 blockPos = new Vector3 (x, y, z);
				CreateBlock (y, blockPos, true);
				//return;
				while (y > 0) {
					y--;
					blockPos = new Vector3 (x, y, z);
					CreateBlock (y, blockPos, false);
				}
			}
		}
	}
}
