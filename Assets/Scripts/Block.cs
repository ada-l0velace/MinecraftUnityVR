using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Block {

	public static ItemTexture Grass = new ItemTexture(new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(3f, 15f), new Vector2(2f, 6f), new Vector2(2f, 15f));
	public static ItemTexture Dirt = new ItemTexture(new Vector2(2f, 15f));
	public static ItemTexture Stone = new ItemTexture(new Vector2(1f, 15f));
	public static ItemTexture Cobblestone = new ItemTexture(new Vector2(0f, 14f));
	public static ItemTexture Bedrock = new ItemTexture(new Vector2(14f, 3f));
	public static ItemTexture TreeTrunk = new ItemTexture(new Vector2(4f, 14f), new Vector2(4f, 14f), new Vector2(4f, 14f), new Vector2(4f, 14f), new Vector2(5f, 14f), new Vector2(5f, 14f));
	public static ItemTexture TreeLeaves = new ItemTexture(new Vector2(1f, 6f));
	public static ItemTexture Diamond = new ItemTexture(new Vector2(2f, 12f));
	public static ItemTexture RedStone = new ItemTexture(new Vector2(3f, 12f));
	public static ItemTexture Water = new ItemTexture(new Vector2(14f, 3f));

	public Material material;

	enum Cubeside {BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK};
	public enum BlockType {GRASS, DIRT, STONE, AIR, LEAVES, WOOD, WOODBASE, DIAMOND, REDSTONE, BEDROCK, WATER};

	public BlockType bType;
	GameObject parent;
	Vector3 position;
	Material cubeMaterial;
	public bool isSolid;
	public Chunk owner;

	public Block(BlockType b, Vector3 pos, GameObject p, Chunk o) {
		bType = b;
		parent = p;
		position = pos;
		owner = o;
		SetType (bType);
	}

	public void SetType(BlockType b) {
		bType = b;
		if (bType == BlockType.AIR || bType == BlockType.WATER)
			isSolid = false;
		else
			isSolid = true;

		if(bType == BlockType.WATER)
		{
			parent = owner.fluid.gameObject;
		}
		else
			parent = owner.chunk.gameObject;
	}

	int ConvertBlockIndexToLocal(int i) {
		if(i <= -1) 
			i = World.chunkSize+i; 
		else if(i >= World.chunkSize) 
			i = i-World.chunkSize;
		return i;
	}

	public Block GetBlock(int x, int y, int z) {
		Block[,,] chunks;

		// Block in a neighbouring chunk
		if (x < 0 || x >= World.chunkSize ||
			y < 0 || y >= World.chunkSize ||
			z < 0 || z >= World.chunkSize) {

			int newX = x, newY = y, newZ = z;
			if (x < 0 || x >= World.chunkSize)
				newX = (x - (int)position.x) * World.chunkSize;
			if (y < 0 || y >= World.chunkSize)
				newY = (y - (int)position.y) * World.chunkSize;
			if (z < 0 || z >= World.chunkSize)
				newZ = (z - (int)position.z) * World.chunkSize;
			
			Vector3 neighbourChunkPos = this.parent.transform.position + new Vector3 (newX,
				newY, newZ);

			string nName = World.BuildChunkName (neighbourChunkPos);

			x = ConvertBlockIndexToLocal (x);
			y = ConvertBlockIndexToLocal (y);
			z = ConvertBlockIndexToLocal (z);

			Chunk nChunk;
			if (World.chunks.TryGetValue (nName, out nChunk))
				chunks = nChunk.chunkData;
			else
				return null;

		} else
			chunks = owner.chunkData;	

		try {
			return chunks [x, y, z];
		} catch (System.IndexOutOfRangeException ex) {}
		return null;
	}


	public bool HasSolidNeighbour(int x, int y, int z) {
		Block b = GetBlock(x,y,z);
		if(b != null)
			return (b.isSolid || b.bType == bType);

		return false;
	}

	void CreateQuad(Cubeside side, ItemTexture texture){
		
		Mesh mesh = new Mesh ();
		mesh.name = "ScriptedMesh";

		Vector3[] vertices = new Vector3[4];
		Vector3[] normals = new Vector3[4];
		Vector2[] uvs = new Vector2[4];
		int[] triangles = new int[6];
		float resolution = 0.0625f;

		//all possible UVs
		Vector2 uv00 = new Vector2(0f,0f) * resolution;
		Vector2 uv10 = new Vector2(1f,0f) * resolution;
		Vector2 uv01 = new Vector2(0f,1f) * resolution;
		Vector2 uv11 = new Vector2(1f,1f) * resolution;

		//all possible vertices
		Vector3 p0 = new Vector3(-0.5f, -0.5f,  0.5f);
		Vector3 p1 = new Vector3( 0.5f, -0.5f,  0.5f);
		Vector3 p2 = new Vector3( 0.5f, -0.5f, -0.5f);
		Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
		Vector3 p4 = new Vector3(-0.5f,  0.5f,  0.5f);
		Vector3 p5 = new Vector3( 0.5f,  0.5f,  0.5f);
		Vector3 p6 = new Vector3( 0.5f,  0.5f, -0.5f);
		Vector3 p7 = new Vector3(-0.5f,  0.5f, -0.5f);


		switch (side) {
			case Cubeside.BOTTOM:
				vertices = new Vector3[] { p0, p1, p2, p3 };
				normals = new Vector3[] {
					Vector3.down,
					Vector3.down,
					Vector3.down,
					Vector3.down
				};

			uvs = new Vector2[] {uv11+texture.bottom,uv01+texture.bottom,uv00+texture.bottom,uv10+texture.bottom};
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };
				break;

			case Cubeside.TOP:
				vertices = new Vector3[] { p7, p6, p5, p4 };
				normals = new Vector3[] {
					Vector3.up,
					Vector3.up,
					Vector3.up,
					Vector3.up
				};

			uvs = new Vector2[] {uv11+ texture.top,uv01+ texture.top,uv00+ texture.top,uv10+ texture.top};
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };
				break;

			case Cubeside.LEFT:
				vertices = new Vector3[] { p7, p4, p0, p3 };
				normals = new Vector3[] {
					Vector3.left,
					Vector3.left,
					Vector3.left,
					Vector3.left
				};

			uvs = new Vector2[] {uv11+ texture.left,uv01+ texture.left,uv00+ texture.left,uv10+ texture.left};
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };
				break;

			case Cubeside.RIGHT:
				vertices = new Vector3[] { p5, p6, p2, p1};
				normals = new Vector3[] {
					Vector3.right,
					Vector3.right,
					Vector3.right,
					Vector3.right
				};

			uvs = new Vector2[] {uv11 + texture.right,uv01+ texture.right,uv00+ texture.right,uv10+ texture.right};
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };
				break;

			case Cubeside.FRONT:
				vertices = new Vector3[] { p4, p5, p1, p0 };
				normals = new Vector3[] {
					Vector3.forward,
					Vector3.forward,
					Vector3.forward,
					Vector3.forward
				};

			uvs = new Vector2[] {uv11+ texture.front,uv01+ texture.front,uv00+ texture.front,uv10+ texture.front};
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };
				break;
			
			case Cubeside.BACK:
				vertices = new Vector3[] { p6, p7, p3, p2 };
				normals = new Vector3[] {
					Vector3.back,
					Vector3.back,
					Vector3.back,
					Vector3.back
				};

			uvs = new Vector2[] { uv11+ texture.back, uv01+ texture.back, uv00+ texture.back, uv10+ texture.back };
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };
				break;
				
				
			default:
				break;
		}


		mesh.vertices = vertices;
		mesh.normals = normals;
		mesh.uv = uvs;
		mesh.triangles = triangles;

		mesh.RecalculateBounds();

		GameObject quad = new GameObject ("quad");
		quad.transform.parent = parent.transform;
		quad.transform.position = position;

		MeshFilter meshFilter = (MeshFilter)quad.AddComponent (typeof(MeshFilter));
		meshFilter.mesh = mesh;

		//MeshRenderer renderer = quad.AddComponent (typeof(MeshRenderer)) as MeshRenderer;
		//renderer.material = cubeMaterial;
		//quad.AddComponent(typeof(MeshCollider)) as MeshCollider;

	}
	// Use this for initialization
	public void Draw () {
		//ItemTexture[] textures = {ItemTexture.Grass};
		if (bType == BlockType.AIR)
			return;
		int[][] b = new int[][] { new int[] {0,-1,0} ,  new int[] {0,1,0} ,  new int[] {-1,0,0} ,  new int[] {1,0,0},  new int[] {0,0,1}, new int[] {0,0,-1} };
		int i = 0;
		foreach (Cubeside side in Enum.GetValues(typeof(Cubeside))) {
			if (!HasSolidNeighbour ((int)position.x + b [i] [0], (int)position.y + b [i] [1], (int)position.z + b [i] [2])) {
				switch (bType) {
				case BlockType.GRASS:
					CreateQuad (side, ItemTexture.Grass);
					break;
				case BlockType.STONE:
					CreateQuad (side, ItemTexture.Stone);
					break;
				case BlockType.DIRT:
					CreateQuad (side, ItemTexture.Dirt);
					break;
				case BlockType.DIAMOND:
					CreateQuad (side, ItemTexture.Diamond);
					break;
				case BlockType.REDSTONE:
					CreateQuad (side, ItemTexture.RedStone);
					break;
				case BlockType.BEDROCK:
					CreateQuad (side, ItemTexture.Bedrock);
					break;
				case BlockType.WATER:
					CreateQuad (side, ItemTexture.Water);
					break;
				case BlockType.WOOD:
					CreateQuad (side, ItemTexture.TreeTrunk);
					break;
				case BlockType.WOODBASE:
					CreateQuad (side, ItemTexture.TreeTrunk);
					break;
				case BlockType.LEAVES:
					CreateQuad (side, ItemTexture.TreeLeaves);
					break;
				default:
					break;
				}
			}
					
			i += 1;
		}
		//CombineQuads();
	}
}
