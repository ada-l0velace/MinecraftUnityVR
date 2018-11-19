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

		for (int z = 0; z < World.chunkSize; z++) {
			for (int y = 0; y < World.chunkSize; y++) {
				for (int x = 0; x < World.chunkSize; x++) {
					
					//int y = (int)(Mathf.PerlinNoise ((x + seed) / detailScale, (z + seed) / detailScale ) * heightScale) * heightOffset;
					//y = World.chunkSize-1;
					Vector3 blockPos = new Vector3 (x, y, z);
					int worldX = (int) (x + chunk.transform.position.x);
					int worldY = (int)(y + chunk.transform.position.y);
					int worldZ = (int) (z + chunk.transform.position.z);
								
					//Debug.Log (y / World.columnHeight);

					if (worldY <= Utils.GenerateHeight(worldX, worldZ))
						chunkData[x,y,z] = new Block (Block.BlockType.GRASS, blockPos, chunk.gameObject, cubeMaterial, this);
					else
						chunkData[x,y,z] = new Block (Block.BlockType.AIR, blockPos, chunk.gameObject, cubeMaterial, this);

				}
			}
		}
	}

	public void DrawChunk() {
		for (int z = 0; z < World.chunkSize; z++) {
			for (int y = 0; y < World.chunkSize; y++) {
				for (int x = 0; x < World.chunkSize; x++) {
					chunkData [x, y, z].Draw ();
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

}
