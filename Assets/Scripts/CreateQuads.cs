using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CreateQuads : MonoBehaviour {

	public Material material;

	enum Cubeside {BOTTOM, TOP, LEFT, RIGHT, FRONT, BACK};

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
		renderer.material = material;

		foreach (Transform quad in this.transform) {
			Destroy (quad.gameObject);
		}
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
		quad.transform.parent = this.gameObject.transform;
		MeshFilter meshFilter = (MeshFilter)quad.AddComponent (typeof(MeshFilter));
		meshFilter.mesh = mesh;
		MeshRenderer renderer = quad.AddComponent (typeof(MeshRenderer)) as MeshRenderer;
		renderer.material = material;
		//quad.AddComponent(typeof(MeshCollider)) as MeshCollider;

	}
	// Use this for initialization
	void Start () {
		//ItemTexture[] textures = {ItemTexture.Grass};
		foreach (Cubeside side in Enum.GetValues(typeof(Cubeside))) {
			CreateQuad (side, ItemTexture.Grass);
		}
		CombineQuads();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
