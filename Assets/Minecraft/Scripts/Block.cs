using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Block {



	public Material material;

	protected enum Cubeside {BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK};
	public enum BlockType {GRASS, DIRT, STONE, AIR, LEAVES, WOOD, WOODBASE, DIAMOND, REDSTONE, BEDROCK, WATER};
	public enum CrackType {NOCRACK, CRACK1, CRACK2, CRACK3, CRACK4}

	public ItemTexture texture;
	public BlockType bType;
	protected GameObject parent;
	public Vector3 position;
	Material cubeMaterial;
	public bool isSolid;
	public Chunk owner;

	public CrackType health;

	public int current_health = 0;
	public int max_health;



	public Block(BlockType b, Vector3 pos, GameObject p, Chunk o) {
		max_health = 4;
		bType = b;
		owner = o;
		parent = p;
		position = pos;
		SetType(bType);
	}

	public virtual void SetType(BlockType b) {
		bType = b;
		if (bType == BlockType.AIR || bType == BlockType.WATER)
			isSolid = false;
		else
			isSolid = true;

		health = CrackType.NOCRACK;
		current_health = max_health;

		if(bType == BlockType.WATER) {
			parent = owner.fluid.gameObject;
		}
		else
			parent = owner.chunk.gameObject;
	}

	public virtual void Reset() {
		health = CrackType.NOCRACK;
		current_health = max_health;
		owner.ReDraw ();
	}

	int ConvertBlockIndexToLocal(int i) {
		if(i <= -1) 
			i = World.chunkSize+i; 
		else if(i >= World.chunkSize) 
			i = i-World.chunkSize;
		return i;
	}

	public bool BuildBlock(Block b) {
		Debug.Log (bType.ToString());
		b.NewBlock(this);
		return true;
	}

	public virtual void NewBlock(Block b) {
		int x = (int)b.position.x;
		int y = (int)b.position.y;
		int z = (int)b.position.z;
		owner.chunkData [x, y, z] = b;
		owner.ReDraw ();
	}

	public Block GetBlock(int x, int y, int z) {
		Block[,,] chunks;

		// Block in a neighbouring chunk
		if (x < 0 || x >= World.chunkSize ||
			y < 0 || y >= World.chunkSize ||
			z < 0 || z >= World.chunkSize) {

			int newX = x, newY = y, newZ = z;
			if (x < 0 || x >= World.chunkSize)
				newX = (x - (int)(position.x)) * World.chunkSize;
			if (y < 0 || y >= World.chunkSize)
				newY = (y - (int)(position.y)) * World.chunkSize;
			if (z < 0 || z >= World.chunkSize)
				newZ = (z - (int)(position.z)) * World.chunkSize;
			
			Vector3 neighbourChunkPos = this.parent.transform.position + new Vector3 (newX,
				newY, newZ);

			string nName = World.BuildChunkName (neighbourChunkPos);

			x = ConvertBlockIndexToLocal (x);
			y = ConvertBlockIndexToLocal (y);
			z = ConvertBlockIndexToLocal (z);

			Chunk nChunk;
			if (World.chunks.TryGetValue (nName, out nChunk))
				chunks = nChunk.chunkData;
			else {
				return null;
			}
		} else
			chunks = owner.chunkData;	

		try {
			return chunks [x, y, z];
		} catch (System.IndexOutOfRangeException ex) { Debug.Log (ex.Message);}
		return null;
	}

	public Block GetBlock2(int x, int y, int z) {
		Block[,,] chunks;

		// Block in a neighbouring chunk
		if (x < 0 || x >= World.chunkSize ||
			y < 0 || y >= World.chunkSize ||
			z < 0 || z >= World.chunkSize) {

			int newX = x, newY = y, newZ = z;
			//if (x < 0 || x >= World.chunkSize)
			newX = (x - (int)(position.x)) * World.chunkSize;
			//if (y < 0 || y >= World.chunkSize)
			newY = (y - (int)(position.y)) * World.chunkSize;
			//if (z < 0 || z >= World.chunkSize)
			newZ = (z - (int)(position.z)) * World.chunkSize;

			Vector3 neighbourChunkPos = this.parent.transform.position + new Vector3 (newX,
				newY, newZ);

			string nName = World.BuildChunkName (neighbourChunkPos);

			x = ConvertBlockIndexToLocal (x);
			y = ConvertBlockIndexToLocal (y);
			z = ConvertBlockIndexToLocal (z);

			Chunk nChunk;
			if (World.chunks.TryGetValue (nName, out nChunk))
				chunks = nChunk.chunkData;
			else {
				return null;
			}
		} else
			chunks = owner.chunkData;	

		try {
			return chunks [x, y, z];
		} catch (System.IndexOutOfRangeException ex) { Debug.Log (ex.Message);}
		return null;
	}

	public virtual bool HitBlock() {
		current_health--;
		health++;

		if (current_health+1 == max_health) {
			owner.mb.StartCoroutine (owner.mb.HealBlock (position));
		}

		if (current_health <= 0) {
			if (HasWaterNeighbour((int)position.x, (int)position.y, (int)position.z)) {
				//owner.chunkData [(int)position.x, (int)position.y, (int)position.z] = new Air (position, owner);
				bType = BlockType.AIR;
				//owner.ReDraw ();
				BuildBlock (new Water (position, owner));
				//return false;
			}
			else {
				owner.chunkData [(int)position.x, (int)position.y, (int)position.z] = new Air (position, owner);
				owner.ReDraw ();
			}
			return true;
		}
		owner.ReDraw ();
		return false;
	}

	public bool HasSolidNeighbour(int x, int y, int z) {
		Block b = GetBlock(x,y,z);
		if(b != null)
			return (b.isSolid || b.bType == bType);

		return false;
	}

	public bool HasWaterNeighbour(int x, int y, int z) {
		int[][] sides = new int[][] {new int[] {0,1,0} ,  new int[] {-1,0,0} ,  new int[] {1,0,0},  new int[] {0,0,1}, new int[] {0,0,-1} };
		for (int i = 0; i < sides.Length; i++) {
			Block b = GetBlock(x+sides[i][0],y+sides[i][1],z+sides[i][2]);
			if(b != null && !b.isSolid && b.bType == Block.BlockType.WATER)
				return true;
		}
		return false;
	}



	protected void CreateQuad(Cubeside side,List<Vector3> v, List<Vector3> n, List<Vector2> u, List<Vector2> su, List<int> t, ItemTexture texture) {
		
		float resolution = 0.0625f;

		//all possible UVs
		Vector2 uv00 = new Vector2(0f,0f) * resolution;
		Vector2 uv10 = new Vector2(1f,0f) * resolution;
		Vector2 uv01 = new Vector2(0f,1f) * resolution;
		Vector2 uv11 = new Vector2(1f,1f) * resolution;


		int x = (int) position.x, y = (int) position.y, z = (int) position.z;

		//all possible vertices
		Vector3 p0 = World.allVertices[x,y,z+1];
		Vector3 p1 = World.allVertices[x+1,y,z+1];
		Vector3 p2 = World.allVertices[x+1,y,z];
		Vector3 p3 = World.allVertices[x,y,z];		 
		Vector3 p4 = World.allVertices[x,y+1,z+1];
		Vector3 p5 = World.allVertices[x+1,y+1,z+1];
		Vector3 p6 = World.allVertices[x+1,y+1,z];
		Vector3 p7 = World.allVertices[x,y+1,z];

		int trioffset = 0;

		switch (side) {
			case Cubeside.BOTTOM:
				trioffset = v.Count;
				v.Add(p0); v.Add(p1); v.Add(p2); v.Add(p3);
				n.Add(World.allNormals[(int)World.NDIR.DOWN]);
				n.Add(World.allNormals[(int)World.NDIR.DOWN]);
				n.Add(World.allNormals[(int)World.NDIR.DOWN]);
				n.Add(World.allNormals[(int)World.NDIR.DOWN]);
				u.Add(uv11+texture.bottom); u.Add(uv01+texture.bottom); u.Add(uv00+texture.bottom); u.Add(uv10+texture.bottom);
				t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
				break;

			case Cubeside.TOP:
				trioffset = v.Count;
				v.Add(p7); v.Add(p6); v.Add(p5); v.Add(p4);
				n.Add(World.allNormals[(int)World.NDIR.UP]);
				n.Add(World.allNormals[(int)World.NDIR.UP]);
				n.Add(World.allNormals[(int)World.NDIR.UP]);
				n.Add(World.allNormals[(int)World.NDIR.UP]);
				u.Add(uv11+texture.top); u.Add(uv01+texture.top); u.Add(uv00+texture.top); u.Add(uv10+texture.top);
				t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
				break;

			case Cubeside.LEFT:
				trioffset = v.Count;
				v.Add(p7); v.Add(p4); v.Add(p0); v.Add(p3);
				n.Add(World.allNormals[(int)World.NDIR.LEFT]);
				n.Add(World.allNormals[(int)World.NDIR.LEFT]);
				n.Add(World.allNormals[(int)World.NDIR.LEFT]);
				n.Add(World.allNormals[(int)World.NDIR.LEFT]);
				u.Add(uv11 + texture.left); u.Add(uv01+ texture.left); u.Add(uv00+ texture.left); u.Add(uv10+ texture.left);
				t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
				break;

			case Cubeside.RIGHT:
				trioffset = v.Count;
				v.Add(p5); v.Add(p6); v.Add(p2); v.Add(p1);
				n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
				n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
				n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
				n.Add(World.allNormals[(int)World.NDIR.RIGHT]);
				u.Add(uv11 + texture.right); u.Add(uv01+ texture.right); u.Add(uv00+ texture.right); u.Add(uv10+ texture.right);
				t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
				break;

			case Cubeside.FRONT:
				trioffset = v.Count;
				v.Add(p4); v.Add(p5); v.Add(p1); v.Add(p0);
				n.Add(World.allNormals[(int)World.NDIR.FRONT]);
				n.Add(World.allNormals[(int)World.NDIR.FRONT]);
				n.Add(World.allNormals[(int)World.NDIR.FRONT]);
				n.Add(World.allNormals[(int)World.NDIR.FRONT]);
				u.Add(uv11 + texture.front); u.Add(uv01+ texture.front); u.Add(uv00+ texture.front); u.Add(uv10+ texture.front);
				t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
				break;
			
			case Cubeside.BACK:
				trioffset = v.Count;
				v.Add(p6); v.Add(p7); v.Add(p3); v.Add(p2);
				n.Add(World.allNormals[(int)World.NDIR.BACK]);
				n.Add(World.allNormals[(int)World.NDIR.BACK]);
				n.Add(World.allNormals[(int)World.NDIR.BACK]);
				n.Add(World.allNormals[(int)World.NDIR.BACK]);
				u.Add(uv11 + texture.back); u.Add(uv01 + texture.back); u.Add(uv00 + texture.back); u.Add(uv10 + texture.back);
				t.Add(1 + trioffset); t.Add(0 + trioffset); t.Add(2 + trioffset); t.Add(3 + trioffset); t.Add(2 + trioffset); t.Add(0 + trioffset);
				break;
				
				
			default:
				break;
		}

		su.Add(uv11+ItemTexture.Cracks[(int)(health)].back); su.Add(uv01+ItemTexture.Cracks[(int)(health)].back); su.Add(uv00+ItemTexture.Cracks[(int)(health)].back);su.Add(uv10+ItemTexture.Cracks[(int)(health)].back);

	}
	// Use this for initialization
	public virtual void Draw (List<Vector3> v, List<Vector3> n, List<Vector2> u, List<Vector2> su, List<int> t, List<Vector3> v_w, List<Vector3> n_w, List<Vector2> u_w, List<Vector2> su_w, List<int> t_w) {
		if (bType == BlockType.AIR)
			return;
		
		int[][] b = new int[][] { new int[] {0,-1,0} ,  new int[] {0,1,0} ,  new int[] {-1,0,0} ,  new int[] {1,0,0},  new int[] {0,0,1}, new int[] {0,0,-1} };
		int i = 0;
		foreach (Cubeside side in Enum.GetValues(typeof(Cubeside))) {
			if (!HasSolidNeighbour ((int)position.x + b [i] [0], (int)position.y + b [i] [1], (int)position.z + b [i] [2])) {
				CreateQuad (side, v, n, u, su, t, texture);
			}
			i += 1;
		}
		//CombineQuads();
	}
}
