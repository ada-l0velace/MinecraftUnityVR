using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.AI;
using UnityEditor;

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

	public bool surfaceChunk = false;
	public bool waterChunk = false;
	public Material cubeMaterial;
	public Material fluidMaterial;
	public Block[,,] chunkData;
	public GameObject chunk;
	public ChunkMB mb;

	public GameObject fluid;
	public enum ChunkStatus {DRAW,DONE,KEEP};
	public ChunkStatus status;
	public float touchedTime;
	BlockData bd;

	List<Vector3> Verts = new List<Vector3>();
	List<Vector3> Norms = new List<Vector3>();
	List<Vector2> UVs = new List<Vector2>();
	List<Vector2> SUVs = new List<Vector2>();
	List<int> Tris = new List<int>();

	List<Vector3> Verts_w = new List<Vector3>();
	List<Vector3> Norms_w = new List<Vector3>();
	List<Vector2> UVs_w = new List<Vector2>();
	List<Vector2> SUVs_w = new List<Vector2>();
	List<int> Tris_w = new List<int> ();

	public bool changed = false;
	bool treesCreated = false;

	string BuildChunkFileName(Vector3 v) {
		return Application.persistentDataPath + "/savedata/chunk_" +
		(int)v.x + "_" +
		(int)v.y + "_" +
		(int)v.z + "_" +
		"_" + World.chunkSize +
		"_" + World.radius +
		".dat";
	}

	bool Load() //read data from file
	{
		/*string chunkFile = BuildChunkFileName(chunk.transform.position);
		if(File.Exists(chunkFile))
		{
			BinaryFormatter bf = new BinaryFormatter();
			FileStream file = File.Open(chunkFile, FileMode.Open);
			bd = new BlockData();
			bd = (BlockData) bf.Deserialize(file);
			file.Close();
			//Debug.Log("Loading chunk from file: " + chunkFile);
			return true;
		}
		*/
		return false;
	}

	public void Save() {
		/*
		string chunkFile = BuildChunkFileName (chunk.transform.position);
		if (!File.Exists(chunkFile)) {
			
			Directory.CreateDirectory (Path.GetDirectoryName (chunkFile));
		}

		BinaryFormatter bf = new BinaryFormatter ();
		FileStream file = File.Open (chunkFile, FileMode.OpenOrCreate);
		bd = new BlockData (chunkData);
		bf.Serialize (file, bd);
		file.Close ();
		*/
	}

	// Use this for initialization
	public Chunk (Vector3 position, Material c, Material t) {
		chunk = new GameObject (World.BuildChunkName(position));
		/*var newFlags = StaticEditorFlags.LightmapStatic | StaticEditorFlags.OccluderStatic | StaticEditorFlags.OccludeeStatic | StaticEditorFlags.BatchingStatic | StaticEditorFlags.NavigationStatic | StaticEditorFlags.OffMeshLinkGeneration;
		GameObjectUtility.SetStaticEditorFlags (chunk, newFlags);
		GameObjectUtility.SetStaticEditorFlags (chunk, GameObjectUtility.GetStaticEditorFlags (chunk) | StaticEditorFlags.NavigationStatic);
		*/
		chunk.transform.position = position;
		fluid = new GameObject (World.BuildChunkName(position) + "_F");
		fluid.transform.position = position;
		mb = chunk.AddComponent<ChunkMB> ();
		fluid.gameObject.tag = "Water";
		(fluid.gameObject.AddComponent<NavMeshObstacle> ()).carving=true;
		//meshSurface = chunk.AddComponent<NavMeshSurface>();
		
		mb.SetOwner (this);
		cubeMaterial = c;
		fluidMaterial = t;
		BuildChunk ();
	}

	void BuildChunk() {
		bool dataFromFile = false;
		dataFromFile = Load();

		chunkData = new Block[World.chunkSize,World.chunkSize,World.chunkSize];
		for(int z = 0; z < World.chunkSize; z++)
			for(int y = 0; y < World.chunkSize; y++)
				for(int x = 0; x < World.chunkSize; x++) {
					Vector3 pos = new Vector3(x,y,z);
					int worldX = (int)(x + chunk.transform.position.x);
					int worldY = (int)(y + chunk.transform.position.y);
					int worldZ = (int)(z + chunk.transform.position.z);

					if(dataFromFile) {
						chunkData[x,y,z] = new Block(bd.matrix[x, y, z], pos, 
							chunk.gameObject, this);
						continue;
					}


					int surfaceHeight = Utils.GenerateHeight(worldX,worldZ);


					if (worldY == 0)
						chunkData [x, y, z] = new Bedrock (pos, this); //new Block(Block.BlockType.BEDROCK, pos, 
							//chunk.gameObject, this);
					else if (Utils.fBM3D (worldX, worldY, worldZ, 0.1f, 3) < 0.42f)
						chunkData [x, y, z] = new Air (pos, this); //new Block(Block.BlockType.AIR, pos, 
							//chunk.gameObject, this);
					else if(worldY <= Utils.GenerateStoneHeight(worldX,worldZ)) {
						if (Utils.fBM3D (worldX, worldY, worldZ, 0.01f, 2) < 0.4f && worldY < 40)
							chunkData [x, y, z] = new Diamond (pos, this); //new Block (Block.BlockType.DIAMOND, pos, 
								//chunk.gameObject, this);
						else if (Utils.fBM3D (worldX, worldY, worldZ, 0.03f, 3) < 0.41f && worldY < 20)
							chunkData [x, y, z] = new RedStone (pos, this); //new Block (Block.BlockType.REDSTONE, pos, 
								//chunk.gameObject, this);
						else
							chunkData [x, y, z] = new Stone (pos, this); //new Block(Block.BlockType.STONE, pos, 
								//chunk.gameObject, this);
					}
					else if(worldY == surfaceHeight) {
						surfaceChunk = true;
						if (Utils.fBM3D (worldX, worldY, worldZ, 0.4f, 2) < 0.4f && worldY > 70)
							chunkData [x, y, z] = new WoodBase (pos, this); //new Block (Block.BlockType.WOODBASE, pos, 
								//chunk.gameObject, this);
						else {
							
							chunkData [x, y, z] = new Grass (pos, this); //new Block (Block.BlockType.GRASS, pos, 
								//chunk.gameObject, this);
						}
					}
					else if(worldY < surfaceHeight)
						chunkData[x,y,z] = new Dirt (pos, this);// new Block(Block.BlockType.DIRT, pos, 
							//chunk.gameObject, this);
					else {
						chunkData [x, y, z] = new Air (pos, this); //new Block(Block.BlockType.AIR, pos, 
							//chunk.gameObject, this);
					}

					if (worldY < 70 && worldY > 40 && chunkData [x, y, z].bType == Block.BlockType.AIR) {
						//fluid.gameObject.transform.parent = World.Instance.transform;
						chunkData [x, y, z] = new Water (pos, this); //new Block (Block.BlockType.WATER, pos, 
							//fluid.gameObject, this);
						waterChunk = true;
					}

					status = ChunkStatus.DRAW;

				}
	}

	void BuildTrees(Block trunk, int x, int y, int z) {
		if (trunk.bType != Block.BlockType.WOODBASE)
			return;

		Block t = trunk.GetBlock (x, y + 1, z);

		if (t != null) {
			//t.SetType (Block.BlockType.WOOD);
			chunkData [x, y+1, z] = new Wood (t.position, this);
			Block t1 = t.GetBlock (x, y + 2, z);
			if (t1 != null) {
				//t1.SetType (Block.BlockType.WOOD);
				chunkData [x, y + 2, z] = new Wood (t1.position, this);
				for (int i = -1; i <= 1; i++)
					for (int j = -1; j <= 1; j++)
						for (int k = 3; k <= 4; k++) {
							Block t2 = trunk.GetBlock (x + i, y + k, z + j);
							if (t2 != null)
								chunkData[x + i, y + k, z + j] = new Leaves (t2.position, this);
								//t2.SetType (Block.BlockType.LEAVES);
							else
								return;
						}
				Block t3 = t1.GetBlock (x, y + 5, z);
				if (t3 != null) {
					chunkData[x, y + 5, z] = new Leaves (t3.position, this);
					//t3.SetType (Block.BlockType.LEAVES);
				}
			}
		}
	}

	public void DrawChunk() {

		Verts.Clear();
		Norms.Clear();
		UVs.Clear();
		SUVs.Clear();
		Tris.Clear();

		Verts_w.Clear();
		Norms_w.Clear();
		UVs_w.Clear();
		SUVs_w.Clear();
		Tris_w.Clear();

		if (!treesCreated) {
			for (int z = 0; z < World.chunkSize; z++)
				for (int y = 0; y < World.chunkSize; y++)
					for (int x = 0; x < World.chunkSize; x++) {
						BuildTrees (chunkData [x, y, z], x, y, z);
					}
			treesCreated = true;
		}

		for (int z = 0; z < World.chunkSize; z++)
			for (int y = 0; y < World.chunkSize; y++)
				for (int x = 0; x < World.chunkSize; x++) {
					
					chunkData [x, y, z].Draw (Verts, Norms, UVs, SUVs, Tris, Verts_w, Norms_w, UVs_w, SUVs_w, Tris_w);

				}

		CombineQuads2 (chunk.gameObject, Verts, Norms, UVs, SUVs, Tris ,cubeMaterial);

		MeshCollider collider = chunk.gameObject.AddComponent (typeof(MeshCollider)) as MeshCollider;
		collider.sharedMesh = chunk.transform.GetComponent<MeshFilter> ().mesh;
		//if(waterChunk)
		CombineQuads2(fluid.gameObject, Verts_w, Norms_w, UVs_w, SUVs_w, Tris_w , fluidMaterial);
		fluid.gameObject.AddComponent<UVScroller> ();
		//CombineQuads(fluid.gameObject, fluidMaterial);

		//CombineTwoMeshes (chunk.gameObject, fluid.gameObject);
		if(waterChunk && chunk.gameObject.GetComponent<NavMeshObstacle>() == null) 
			chunk.gameObject.AddComponent<NavMeshObstacle>();
		status = ChunkStatus.DONE;
		/*if (surfaceChunk) {
			meshSurface = chunk.AddComponent<NavMeshSurface>();
			var newFlags = StaticEditorFlags.OffMeshLinkGeneration | StaticEditorFlags.NavigationStatic;
			GameObjectUtility.SetStaticEditorFlags(chunk, newFlags);
			//StaticEditorFlags.OffMeshLinkGeneration = true;
			meshSurface.BuildNavMesh ();
		}*/
		//meshSurface.BuildNavMesh ();
		//World.Instance.wtf++;
		//Debug.Log(World.Instance.wtf);
		if (World.Instance.firstbuild) {
			World.Instance.processCount++;
			World.Instance.loadingAmount.value = World.Instance.processCount / (World.totalChunks);
		}
	}

	public void ReDraw() {
		GameObject.DestroyImmediate (chunk.GetComponent<MeshFilter> ());
		GameObject.DestroyImmediate (chunk.GetComponent<MeshRenderer> ());
		GameObject.DestroyImmediate (chunk.GetComponent<Collider> ());

		GameObject.DestroyImmediate (fluid.GetComponent<MeshFilter> ());
		GameObject.DestroyImmediate (fluid.GetComponent<MeshRenderer> ());
		DrawChunk ();
	}

	public void CombineChildren(GameObject o, Material m) {
		//1. Combine all children meshes
		MeshFilter[] meshFilters = o.GetComponentsInChildren<MeshFilter>();
		CombineInstance[] combine = new CombineInstance[meshFilters.Length];
		int i = 0;
		while (i < meshFilters.Length) {
			combine[i].mesh = meshFilters[i].sharedMesh;
			combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
			i++;
		}

		//2. Create a new mesh on the parent object
		MeshFilter mf = (MeshFilter) o.gameObject.AddComponent(typeof(MeshFilter));
		mf.mesh = new Mesh();

		//3. Add combined meshes on children as the parent's mesh
		mf.mesh.CombineMeshes(combine);

		//4. Create a renderer for the parent
		MeshRenderer renderer = o.gameObject.AddComponent(typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = m;

		//5. Delete all uncombined children
		foreach (Transform quad in o.transform) {
			GameObject.Destroy(quad.gameObject);
		}

	}

	public void CombineQuads2(GameObject o, List<Vector3> v, List<Vector3> n, List<Vector2> u, List<Vector2> su, List<int> t, Material m) {
		Mesh mesh = new Mesh();
		mesh.name = "ScriptedMesh"; 

		mesh.vertices = v.ToArray();
		mesh.normals = n.ToArray();
		mesh.uv = u.ToArray();
		mesh.SetUVs(1, su);
		mesh.triangles = t.ToArray();


		mesh.RecalculateBounds();
		mesh.RecalculateNormals ();

		MeshFilter meshFilter = (MeshFilter) o.AddComponent<MeshFilter>();
		meshFilter.mesh = mesh;

		MeshRenderer renderer = o.AddComponent<MeshRenderer>();
		renderer.material = m;

	}

}
