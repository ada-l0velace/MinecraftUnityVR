using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

public class WoodBase : Wood {

	public WoodBase(Vector3 pos, Chunk o) : base(pos, o) {
		bType = Block.BlockType.WOODBASE;
		audioClips = ResourcesManager.Instance.WoodAudio;
	}
		
}
