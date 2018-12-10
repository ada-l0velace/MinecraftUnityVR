using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RedStone : Block {

	public RedStone(Vector3 pos, Chunk o) : base(BlockType.REDSTONE, pos, o.chunk.gameObject, o) {
		texture = ItemTexture.RedStone;
		audioClips = ResourcesManager.Instance.StoneAudio;
	}

}
