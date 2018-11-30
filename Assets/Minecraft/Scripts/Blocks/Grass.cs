using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Grass : Block {

	public Grass(Vector3 pos, Chunk o) : base(BlockType.GRASS, pos, o.chunk.gameObject, o) {
		texture = ItemTexture.Grass;
	}

	/*public override void SetType(BlockType b) {
		bType = b;
		isSolid = true;
		parent = owner.chunk.gameObject;
		//texture = ItemTexture.Grass;
	}*/


}
