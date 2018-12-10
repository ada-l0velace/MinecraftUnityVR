using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationCreep : MonoBehaviour {
	Character character {
		get { return World.Instance.character; }
		set { World.Instance.character = value; }
	}

	Mob _mob {
		get { return World.Instance.mob; }
		set { World.Instance.mob = value; }
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	public void AttackEnd() {
		_mob.attack(character);
		Destroy(character.RemoveHeart ());
	}
}
