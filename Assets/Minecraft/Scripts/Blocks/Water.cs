using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Water : Block {
	
	public Water(Vector3 pos, Chunk o) : base(BlockType.WATER, pos, o.fluid.gameObject, o) {
		texture = ItemTexture.Water;
	}

	public override void Draw (List<Vector3> v, List<Vector3> n, List<Vector2> u, List<Vector2> su, List<int> t, List<Vector3> v_w, List<Vector3> n_w, List<Vector2> u_w, List<Vector2> su_w, List<int> t_w) {
		int[][] b = new int[][] { new int[] {0,-1,0} ,  new int[] {0,1,0} ,  new int[] {-1,0,0} ,  new int[] {1,0,0},  new int[] {0,0,1}, new int[] {0,0,-1} };
		int i = 0;
		foreach (Cubeside side in Enum.GetValues(typeof(Cubeside))) {
			if (!HasSolidNeighbour ((int)position.x + b [i] [0], (int)position.y + b [i] [1], (int)position.z + b [i] [2])) {
				CreateQuad (side, v_w, n_w, u_w, su_w, t_w, texture);
			}
			i += 1;
		}
	}
}
