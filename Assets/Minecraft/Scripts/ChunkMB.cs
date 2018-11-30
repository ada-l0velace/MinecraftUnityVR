using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkMB : MonoBehaviour {
	Chunk owner;

	public ChunkMB() {}

	public void SetOwner(Chunk o) {
		owner = o;
	}

	public IEnumerator HealBlock(Vector3 bpos) {
		yield return new  WaitForSeconds(3);
		int x = (int)bpos.x;
		int y = (int)bpos.y;
		int z = (int)bpos.z;

		owner.chunkData[x, y,z].Reset();

	}
}
