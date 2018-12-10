using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Wood : Block {

	public Wood(Vector3 pos, Chunk o) : base(BlockType.WOOD, pos, o.chunk.gameObject, o) {
		texture = ItemTexture.TreeTrunk;
		audioClips = ResourcesManager.Instance.WoodAudio;
	}
}
