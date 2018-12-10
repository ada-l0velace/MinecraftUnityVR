using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Diamond : Block {

	public Diamond(Vector3 pos, Chunk o) : base(BlockType.DIAMOND, pos, o.chunk.gameObject, o) {
		texture = ItemTexture.Diamond;
		audioClips = ResourcesManager.Instance.StoneAudio;
	}

}
