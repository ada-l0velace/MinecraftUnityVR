using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Air : Block {

	public Air(Vector3 pos, Chunk o) : base(BlockType.AIR, pos, o.chunk.gameObject, o) {
		//texture = ItemTexture.;
	}

	public override void Reset() {
		return;
	}

	public override void Draw (List<Vector3> v, List<Vector3> n, List<Vector2> u, List<Vector2> su, List<int> t, List<Vector3> v_w, List<Vector3> n_w, List<Vector2> u_w, List<Vector2> su_w, List<int> t_w) {
		return;
	}
}
