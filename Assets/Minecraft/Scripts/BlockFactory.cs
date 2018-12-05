using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static class BlockFactory {
	/// <summary>
	/// Decides which block to instantiate.
	/// </summary>


	public static Block Get(Block.BlockType blockType, Vector3 pos, Chunk o) {
		switch (blockType) {
			case Block.BlockType.GRASS:
				return new Grass (pos, o);
			case Block.BlockType.DIRT:
				return new Dirt(pos, o);
			case Block.BlockType.STONE:
				return new Stone(pos, o);
			case Block.BlockType.AIR:
				return new Air(pos, o);
			case Block.BlockType.LEAVES:
				return new Leaves(pos, o);
			case Block.BlockType.WOOD:
				return new Wood(pos, o);
			case Block.BlockType.WOODBASE:
				return new WoodBase(pos, o);
			case Block.BlockType.DIAMOND:
				return new Diamond(pos, o);
			case Block.BlockType.REDSTONE:
				return new RedStone(pos, o);
			case Block.BlockType.BEDROCK:
				return new Bedrock(pos, o);
			case Block.BlockType.WATER:
				return new Water(pos, o);
			default:
				return null;
		}
	}
}