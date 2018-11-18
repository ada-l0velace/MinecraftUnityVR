using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chunks : MonoBehaviour {
	public Material cubeMaterial;

	// Use this for initialization
	void Start () {
		StartCoroutine (GenerateChunk (30, 30, 30));
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void CreateBlock(int y, Vector3 blockPos, bool create) {
		//Debug.Log (blockPos.x + " " + blockPos.y + " " + blockPos.z);
		if (create) {
			Block b = new Block (Block.BlockType.GRASS, blockPos, this.gameObject, cubeMaterial);
			b.Draw();
			//Instantiate (Grass_block, blockPos, Quaternion.identity);
			//block.transform.parent = this.transform;
			//Combine (block);
		}
		//else 
		//	Instantiate (Dirt_block, blockPos, Quaternion.identity);
	}

	void CombineQuads() {
		MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine [i].transform = meshFilters [i].transform.localToWorldMatrix;
			i++;
		}

		MeshFilter mf = (MeshFilter)gameObject.AddComponent (typeof(MeshFilter));


		mf.mesh = new Mesh ();
		mf.mesh.CombineMeshes (combine);

		MeshRenderer renderer = this.gameObject.AddComponent (typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = cubeMaterial;

		MeshCollider collider = this.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		foreach (Transform quad in this.transform) {
			Destroy (quad.gameObject);
		}
	}

	IEnumerator GenerateChunk (int width, int height, int depth) {
		//int depth = 30;
		//int width = 30;
		//int height = 3;
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
			yield return null;
		}
		CombineQuads ();
	}

}
