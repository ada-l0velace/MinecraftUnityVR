using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryManager : MonoBehaviour {
	//int slotAmount = 9;
	public GameObject hotbarPanel;
	public GameObject background;
	public GameObject slot;
	public Material material;

	public int selectedItem {
		get { return World.Instance.character.inventory.selectedItem; }
		set { World.Instance.character.inventory.selectedItem = value; }
	}
	int slotAmount {
		get { return World.Instance.character.inventory.slotAmount; }
		set { slotAmount = value; }
	}

	public Inventory inventory {
		get { return World.Instance.character.inventory; }
		set { World.Instance.character.inventory = value; }
	}

	Color unselected = new Color32 (0x9d, 0x9d, 0x9d, 0xff);
	Color selected = Color.white;//new Color32(0x5f,0x98,0x1e, 0xff);


	public List<Minecraft.Item> slots {
		get { return World.Instance.character.inventory.slots; }
		set { World.Instance.character.inventory.slots = value; }
	}

	// Use this for initialization
	void Start () {
		inventory = new Inventory(9, hotbarPanel);
		Block.BlockType[] blockTypes = new Block.BlockType[] {Block.BlockType.GRASS, Block.BlockType.DIRT, Block.BlockType.STONE, Block.BlockType.WOOD, Block.BlockType.WATER, Block.BlockType.LEAVES, Block.BlockType.GRASS, Block.BlockType.GRASS, Block.BlockType.GRASS };
		Vector2[] items = new Vector2[] { new Vector2(16, 8), new Vector2(14, 8), new Vector2(12, 8), new Vector2(0, 7), new Vector2(0, 6), new Vector2(24, 7), new Vector2(16, 8), new Vector2(16, 8), new Vector2(16, 8) }; 
		//slotAmount = World.Instance.character.inventory.slotAmount;
		 
		for (int i = 0; i < slotAmount; i++) {
			Minecraft.Item item = new BlockItem((int)items[i].x, (int)items[i].y, Instantiate(slot), blockTypes[i], material);
			inventory.addItemSelectionBar(item, Instantiate(background));
			
		}
		slots[0].inventoryIcon.GetComponentInChildren<RawImage> ().color = selected;
	}
	
	// Update is called once per frame
	void Update () {
		var axis = Input.GetAxis ("Mouse ScrollWheel");
		bool up = axis > 0f;
		bool down = axis < 0f;

		if (up) {
			slots[selectedItem].inventoryIcon.GetComponentInChildren<RawImage> ().color = unselected;
			selectedItem = (selectedItem < (slotAmount-1)) ? (selectedItem + 1) : 0;
			slots[selectedItem].inventoryIcon.GetComponentInChildren<RawImage> ().color = selected;
		}
		else if (down) {
			slots[selectedItem].inventoryIcon.GetComponentInChildren<RawImage> ().color = unselected;
			selectedItem = (selectedItem == 0) ? (slotAmount-1) : (selectedItem - 1);
			slots[selectedItem].inventoryIcon.GetComponentInChildren<RawImage> ().color = selected;
		}
	}
}
