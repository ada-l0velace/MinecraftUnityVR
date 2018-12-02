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

	public IEnumerator Flow(Block b, Block old_b, Block.BlockType bt, int strength, int maxsize) {
		//reduce the strength of the fluid block
		//with each new block created
		if(maxsize <=0) yield break;
		if (b == null) yield break;
		if (strength <= 0) yield break;
		if (old_b.bType != Block.BlockType.AIR) yield break;
		//b.BuildBlock ();
		b.current_health = strength;
		old_b.NewBlock(b);

		int x = (int)b.position.x;
		int y = (int)b.position.y;
		int z = (int)b.position.z;
		yield return new WaitForSeconds (1);

		Block below = b.GetBlock2(x, y - 1, z);
		if (below != null && below.bType == Block.BlockType.AIR) {
			StartCoroutine (Flow (new Water (below.position, below.owner), below, bt, strength, --maxsize));
			yield break;
		} 
		else if (below != null) {
			
			if (below == null) {
				Debug.Log (z+ " " + y + " " + x);
			}
			--strength;
			--maxsize;

			Block leftward = b.GetBlock2 (x - 1, y, z);
			if (leftward != null) {
				// flow left
				World.Instance.queue.Run (Flow (new Water (leftward.position, leftward.owner), leftward, bt, strength, maxsize));
				yield return new WaitForSeconds (1);
			}

			Block rightward = b.GetBlock2 (x + 1, y, z);
			if (rightward != null) {
				// flow right
				World.Instance.queue.Run (Flow (new Water (rightward.position, rightward.owner), rightward, bt, strength, maxsize));
				yield return new WaitForSeconds (1);
			}
			Block forward = b.GetBlock2(x, y, z+1);
			if (forward != null) {
				// flow forward
				World.Instance.queue.Run (Flow (new Water (forward.position, forward.owner), forward, bt, strength, maxsize));
				yield return new WaitForSeconds (1);
			}

			Block backward = b.GetBlock2(x, y, z-1);
			//Debug.Log (backward);
			//Debug.Log (x.ToString () + " " + y.ToString () + " " + (z - 1).ToString ());
			if (backward != null) {
				// flow backward
				World.Instance.queue.Run (Flow (new Water (backward.position, backward.owner), backward, bt, strength, maxsize));
				yield return new WaitForSeconds (1);
			}
		}
	}
}
