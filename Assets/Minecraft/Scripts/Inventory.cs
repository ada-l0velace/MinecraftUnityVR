using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory {

	public int selectedItem;
	public int slotAmount;
	public List<Minecraft.Item> slots = new List<Minecraft.Item>();
	public GameObject selectionBar;

	public Inventory(int mainItemsSlots, GameObject panel) {
		selectedItem = 0;
		slotAmount = mainItemsSlots;
		selectionBar = panel;
	}

	public void addItemSelectionBar(Minecraft.Item i, GameObject Background) {
		i.inventoryIcon.transform.SetParent(selectionBar.transform, false);
		i.inventoryIcon.transform.localScale = new Vector3(80, 80, 1);
		i.inventoryIcon.name = "slot" + slots.Count.ToString();
		Background.transform.SetParent(i.inventoryIcon.transform, false);
		slots.Add(i);
	}

	public Minecraft.Item getSelectedItem() {
		return slots[selectedItem];
	}

}
