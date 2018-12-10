using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Leaves : Block {

	public Leaves(Vector3 pos, Chunk o) : base(BlockType.LEAVES, pos, o.chunk.gameObject, o) {
		texture = ItemTexture.TreeLeaves;
		audioClips = ResourcesManager.Instance.GrassAudio;
	}
}
