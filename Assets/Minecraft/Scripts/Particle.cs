using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle : MonoBehaviour {
	public static ParticleSystem particles;
	public Material material;
	// Use this for initialization
	void Start () {
		particles = GetComponent<ParticleSystem> ();
		//ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
		//CreateCube(ItemTexture.Bedrock);
		//renderer.material =
	}

	public static ParticleSystem GetParticle() {
		return particles;
	}
	// Update is called once per frame
	void Update () {
		
	}

	protected void CreateCube(ItemTexture texture) {

		float resolution = 0.0625f;

		//all possible UVs
		Vector2 uv00 = new Vector2(0f,0f) * resolution;
		Vector2 uv10 = new Vector2(1f,0f) * resolution;
		Vector2 uv01 = new Vector2(0f,1f) * resolution;
		Vector2 uv11 = new Vector2(1f,1f) * resolution;

		float size = 1f;
		Vector3[] vertices = {
			new Vector3(0, size, size),
			new Vector3(size, size, size), // front
			new Vector3(size, 0, size),
			new Vector3(0, 0, size),

			new Vector3(size, size, 0), //back
			new Vector3(0, size, 0),
			new Vector3(0, 0, 0),
			new Vector3(size, 0, 0),

			new Vector3(0, size, 0), //top
			new Vector3(size, size, size),
			new Vector3(size, size, 0),
			new Vector3(0, size, size),

			new Vector3(0, 0, size), // bot
			new Vector3(size, 0, size),
			new Vector3(size, 0, 0),
			new Vector3(0, 0, 0),

			new Vector3(0, size, 0), // left
			new Vector3(0, size, size),
			new Vector3(0, 0, size),
			new Vector3(0, 0, 0),

			new Vector3(size, size, size), //right
			new Vector3(size, size, 0),
			new Vector3(size, 0, 0),
			new Vector3(size, 0, size),

		};


		Vector3[] normals = {
			Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
			Vector3.back, Vector3.back, Vector3.back, Vector3.back,
			Vector3.up, Vector3.up, Vector3.up, Vector3.up,
			Vector3.down, Vector3.down, Vector3.down, Vector3.down,
			Vector3.left, Vector3.left, Vector3.left, Vector3.left,
			Vector3.right, Vector3.right, Vector3.right, Vector3.right,
		};

		int[] triangles = {
			1, 3, 0, // front
			1, 2, 3,

			4, 7, 5, // back
			5, 7, 6,

			8, 9, 10, //top
			9, 8, 11, 

			15, 13, 12, //bottom
			13, 15, 14,

			18, 16, 19,// left
			16, 18, 17,
			
			20, 22, 21,//right
			22, 20, 23
		};

		Vector2[] uvs = {
			uv11+texture.front,
			uv01+texture.front,
			uv00+texture.front,
			uv10+texture.front,

			uv11+texture.back,
			uv01+texture.back,
			uv00+texture.back,
			uv10+texture.back,

			uv11+texture.top,
			uv01+texture.top,
			uv00+texture.top,
			uv10+texture.top,

			uv11+texture.bottom,
			uv01+texture.bottom,
			uv00+texture.bottom,
			uv10+texture.bottom,

			uv11+texture.left,
			uv01+texture.left,
			uv00+texture.left,
			uv10+texture.left,

			uv11+texture.right,
			uv01+texture.right,
			uv00+texture.right,
			uv10+texture.right,


		}; 
		Mesh mesh = new Mesh();
		mesh.Clear ();
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.uv = uvs;
		mesh.RecalculateBounds();
		mesh.RecalculateNormals ();
		particles.GetComponent<ParticleSystemRenderer> ().mesh = mesh;

	}
}
