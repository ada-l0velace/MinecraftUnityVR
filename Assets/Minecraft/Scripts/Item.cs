using System.Collections;
using System.Collections.Generic;
using UnityEngine;

interface IItem {
	Block.BlockType getBlockType();
}
namespace Minecraft {
	public class Item : IItem {

		public GameObject inventoryIcon;
		int textureX;
		int textureY;
		public int damage;
		public Material material;

		public Item(int xTexture, int yTexture, GameObject inventorySlot, Material m) {
			textureX = xTexture;
			textureY = yTexture;
			inventoryIcon = inventorySlot;
			material = m;
			inventoryIcon = CreateQuad (textureX, textureY);
		}

		public virtual Block.BlockType getBlockType() {
			return Block.BlockType.AIR;
		}

		GameObject CreateQuad (int x, int y) {
			Mesh mesh = new Mesh ();
			mesh.name = "ScriptedMesh";

			Vector3[] vertices = new Vector3[4];
			Vector3[] normals = new Vector3[4];
			Vector2[] uvs = new Vector2[4];
			int[] triangles = new int[6];

			Vector2 uv00 = new Vector2(0f+0.03125f*x,0f+0.01298701f*y);
			Vector2 uv10 = new Vector2(0.03125f*(x+1),0f+0.01298701f*y);
			Vector2 uv01 = new Vector2(0f+0.03125f*x,0.01298701f*(y+1));
			Vector2 uv11 = new Vector2(0.03125f*(x+1),0.01298701f*(y+1));

			Vector3 p2 = new Vector3( 0.5f, -0.5f, -0.5f);
			Vector3 p3 = new Vector3(-0.5f, -0.5f, -0.5f);
			Vector3 p6 = new Vector3( 0.5f,  0.5f, -0.5f);
			Vector3 p7 = new Vector3(-0.5f,  0.5f, -0.5f);

			vertices = new Vector3[] {p6, p7, p3, p2};
			normals = new Vector3[] {Vector3.back, Vector3.back, 
				Vector3.back, Vector3.back};
			uvs = new Vector2[] {uv11, uv01, uv00, uv10};
			triangles = new int[] {3, 1, 0, 3, 2, 1};


			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.uv = uvs;
			mesh.triangles = triangles;

			mesh.RecalculateBounds();
			mesh.RecalculateNormals ();

			GameObject quad = inventoryIcon;

			MeshFilter meshFilter = (quad.GetComponent<MeshFilter> () == null) ? (MeshFilter)quad.AddComponent (typeof(MeshFilter)) : quad.GetComponent<MeshFilter>();
			meshFilter.mesh = mesh;

			MeshRenderer renderer = (quad.GetComponent<MeshRenderer> () == null) ? quad.AddComponent (typeof(MeshRenderer)) as MeshRenderer : quad.GetComponent<MeshRenderer>();
			renderer.material = material;

			//quad.transform.SetParent(hotbarPanel.transform, false);
			return quad;	
		}
	}
}
