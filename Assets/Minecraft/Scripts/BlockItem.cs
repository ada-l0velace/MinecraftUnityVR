using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockItem : Minecraft.Item {

	public Block.BlockType btype;

	public BlockItem(int xTexture, int yTexture, GameObject inventorySlot, Block.BlockType type, Material m) : base(xTexture, yTexture, inventorySlot, m) {
		btype = type;
	}

	public override Block.BlockType getBlockType() {
		return btype;
	}
}
