using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Dirt : Block {

	public Dirt(Vector3 pos, Chunk o) : base(BlockType.DIRT, pos, o.chunk.gameObject, o) {
		texture = ItemTexture.Dirt;
		audioClips = ResourcesManager.Instance.GrassAudio;
	}



}
