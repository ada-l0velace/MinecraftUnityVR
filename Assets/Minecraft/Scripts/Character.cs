using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character {

	public Inventory inventory;
	public AudioSource audio;
	GameObject healthPanel;
	List<RawImage> hearts = new List<RawImage>();
	public List<AudioClip> sounds;
	Animator animator;
	public int currentHealth;
	int maxHealth;
	int baseDamage = 1;


	public Character(GameObject healthP) {
		maxHealth = 8;
		currentHealth = maxHealth;
		healthPanel = healthP;

	}

	public void attack (Character c) {
		c.takingDamage(baseDamage);
	}

	public void takingDamage(int damage) {
		audio.clip = sounds[0];
		audio.PlayOneShot(audio.clip);
		audio.Play();
		currentHealth = currentHealth - damage;
	}

	public bool isCharacterDead() {
		return currentHealth <= 0;
	}

	public void BuildHeart(RawImage heart) {
		heart.transform.SetParent (healthPanel.transform, false);
		hearts.Add(heart);
	}

	public RawImage RemoveHeart() {
		RawImage heart = hearts[hearts.Count - 1];
		hearts.RemoveAt(hearts.Count - 1);
		return heart;
	}

}
