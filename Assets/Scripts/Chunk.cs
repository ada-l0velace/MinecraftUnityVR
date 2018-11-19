using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chunk {
	public Material cubeMaterial;
	public Block[,,] chunkData;
	public GameObject chunk;

	// Use this for initialization
	public Chunk (Vector3 position, Material c) {
		chunk = new GameObject (World.BuildChunkName(position));
		chunk.transform.position = position;
		cubeMaterial = c;
		BuildChunk ();
	}

	void BuildChunk() {
		chunkData = new Block[World.chunkSize, World.chunkSize, World.chunkSize];
		int heightScale = World.chunkSize-1;
		int heightOffset = 1;
		float detailScale = 25.0f;
		int seed = Random.Range(100000, 999999);
		for (int z = 0; z < World.chunkSize; z++) {
			for (int x = 0; x < World.chunkSize; x++) {
				int y = (int)(Mathf.PerlinNoise ((x + seed) / detailScale, (z + seed) / detailScale ) * heightScale) * heightOffset;
				//y = World.chunkSize-1;
				//Mathf.Round (transform.position.x, MidpointRounding.AwayFromZero);
				//Mathf.Round (transform.position.y, MidpointRounding.AwayFromZero);
				//Mathf.Round (transform.position.z+1, MidpointRounding.AwayFromZero);
				Vector3 blockPos = new Vector3 (x, y, z);
				//CreateBlock (y, blockPos, true);
				chunkData[x,y,z] = new Block (Block.BlockType.GRASS, blockPos, chunk.gameObject, cubeMaterial, this);

				//return;
				while (y > 0) {
					y--;
					blockPos = new Vector3 (x, y, z);
					chunkData[x,y,z] = new Block (Block.BlockType.GRASS, blockPos, chunk.gameObject, cubeMaterial, this);
					//CreateBlock (y, blockPos, false);

				}

			}
		}
	}

	void DrawChunk() {
		for (int z = 0; z < World.chunkSize; z++) {
			for (int x = 0; x < World.chunkSize; x++) {
				for (int y = 0; y < World.chunkSize; y++) {
					Vector3 blockPos = new Vector3 (x, y, z);
					if (chunkData [x, y, z] != null) {
						chunkData [x, y, z].Draw ();
					}
				}
			}
		}
		CombineQuads ();
	}

	void CombineQuads() {
		MeshFilter[] meshFilters = chunk.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine [i].transform = meshFilters [i].transform.localToWorldMatrix;
			i++;
		}

		MeshFilter mf = (MeshFilter)chunk.gameObject.AddComponent (typeof(MeshFilter));


		mf.mesh = new Mesh ();
		mf.mesh.CombineMeshes (combine);

		MeshRenderer renderer = chunk.gameObject.AddComponent (typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = cubeMaterial;

		MeshCollider collider = chunk.gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
		foreach (Transform quad in chunk.transform) {
			GameObject.Destroy (quad.gameObject);
		}
	}

	public void GenerateChunk () {
		//int depth = 30;
		//int width = 30;
		//int height = 3;
		BuildChunk();
		DrawChunk ();
	}

}
