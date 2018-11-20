using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;


[Serializable]
class BlockData {
	public Block.BlockType[,,] matrix;

	public BlockData() {}

	public BlockData(Block[,,] b) {
		matrix = new Block.BlockType[World.chunkSize, World.chunkSize, World.chunkSize];
		for (int z = 0; z < World.chunkSize; z++) {
			for (int y = 0; y < World.chunkSize; y++) {
				for (int x = 0; x < World.chunkSize; x++) {
					matrix [x, y, z] = b[x, y, z].bType;
				}
			}
		}
	}
}

public class Chunk {
	public Material cubeMaterial;
	public Block[,,] chunkData;
	public GameObject chunk;
	public enum ChunkStatus {DRAW,DONE,KEEP};
	public ChunkStatus status;
	public float touchedTime;
	BlockData bd;

	string BuildChunkFileName(Vector3 v) {
		return Application.persistentDataPath + "/savedata/chunk_" +
		(int)v.x + "_" +
		(int)v.y + "_" +
		(int)v.z + "_" +
		"_" + World.chunkSize +
		"_" + World.radius +
		".dat";
	}

	bool Load() {
		string chunkFile = BuildChunkFileName (chunk.transform.position);
		if (File.Exists (chunkFile)) {
			BinaryFormatter bf = new BinaryFormatter ();
			FileStream file = File.Open (chunkFile, FileMode.Open);
			bd = new BlockData ();
			bd = (BlockData)bf.Deserialize (file);
			file.Close ();
			return true;
		}
		return false;
	}

	public void Save() {
		string chunkFile = BuildChunkFileName (chunk.transform.position);
		if (!File.Exists(chunkFile)) {
			
			Directory.CreateDirectory (Path.GetDirectoryName (chunkFile));
		}

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (chunkFile, FileMode.OpenOrCreate);
		bd = new BlockData (chunkData);
		bf.Serialize (file, bd);
		file.Close ();
	}

	// Use this for initialization
	public Chunk (Vector3 position, Material c) {
		chunk = new GameObject (World.BuildChunkName(position));
		chunk.transform.position = position;
		cubeMaterial = c;
		BuildChunk ();
	}

	void BuildChunk() {
		bool dataFromFile = false;
		dataFromFile = Load ();
		touchedTime = Time.time;
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
					int surfaceHeight = Utils.GenerateHeight(worldX,worldZ);			
					//Debug.Log (y / World.columnHeight);

					if (dataFromFile) {
						chunkData [x, y, z] = new Block (bd.matrix [x, y, z], blockPos, chunk.gameObject, cubeMaterial, this);
						continue;
					}

					if (worldY == 0)
						chunkData [x, y, z] = new Block (Block.BlockType.BEDROCK, blockPos, chunk.gameObject, cubeMaterial, this);
					else if (Utils.fBM3D (worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
						chunkData [x, y, z] = new Block (Block.BlockType.AIR, blockPos, chunk.gameObject, cubeMaterial, this);
					else if (worldY <= Utils.GenerateStoneHeight (worldX, worldZ)) {
						if (Utils.fBM3D (worldX, worldY, worldZ, 0.2f, 2) < 0.38f && worldY < 40)
							chunkData [x, y, z] = new Block (Block.BlockType.DIAMOND, blockPos, chunk.gameObject, cubeMaterial, this);
						else if (Utils.fBM3D (worldX, worldY, worldZ, 0.03f, 3) < 0.41f && worldY < 20)
							chunkData [x, y, z] = new Block (Block.BlockType.REDSTONE, blockPos, chunk.gameObject, cubeMaterial, this);
						else
							chunkData [x, y, z] = new Block (Block.BlockType.STONE, blockPos, chunk.gameObject, cubeMaterial, this);
					}
					else if (worldY == Utils.GenerateHeight(worldX, worldZ))
						chunkData[x,y,z] = new Block (Block.BlockType.GRASS, blockPos, chunk.gameObject, cubeMaterial, this);
					else if (worldY < Utils.GenerateHeight(worldX, worldZ))
						chunkData[x,y,z] = new Block (Block.BlockType.DIRT, blockPos, chunk.gameObject, cubeMaterial, this);
					else
						chunkData[x,y,z] = new Block (Block.BlockType.AIR, blockPos, chunk.gameObject, cubeMaterial, this);
					status = ChunkStatus.DRAW;
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
		collider.sharedMesh = chunk.transform.GetComponent<MeshFilter>().mesh;

		foreach (Transform quad in chunk.transform) {
			GameObject.Destroy (quad.gameObject);
		}
		status = ChunkStatus.DONE;
	}

}
