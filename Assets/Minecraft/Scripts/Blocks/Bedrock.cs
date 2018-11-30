using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Bedrock : Block {

	public Bedrock(Vector3 pos, Chunk o) : base(BlockType.BEDROCK, pos, o.chunk.gameObject, o) {
		max_health = 4;
		texture = ItemTexture.Bedrock;
	}

}
