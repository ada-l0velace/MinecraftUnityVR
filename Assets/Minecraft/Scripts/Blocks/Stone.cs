using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Stone : Block {

	public Stone(Vector3 pos, Chunk o) : base(BlockType.STONE, pos, o.chunk.gameObject, o) {
		texture = ItemTexture.Stone;
	}

}
