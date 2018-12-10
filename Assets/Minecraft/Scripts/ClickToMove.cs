using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.SceneManagement;
// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
	public Transform player;
	public Transform mob;
	public NavMeshAgent m_Agent;
	Vector3 startPosition;
	bool commingBack = true;
	Character character {
		get { return World.Instance.character; }
		set { World.Instance.character = value; }
	}

	Mob _mob {
		get { return World.Instance.mob; }
		set { World.Instance.mob = value; }
	}


	void Start() {
		startPosition = transform.position;
		m_Agent = GetComponent<NavMeshAgent>();
		InvokeRepeating("PlaySound", 2.0f, 5.0f);
	}

	void PlaySound() {
		AudioSource audioSource = GetComponent<AudioSource> ();
		audioSource.Play();
	}
	void Update() {
		/*if (Input.GetMouseButtonDown(0) && !Input.GetKey(KeyCode.LeftShift))
		{
			var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray.origin, ray.direction, out m_HitInfo))*/
		if (World.Instance.meshSurface != null && Vector3.Distance (startPosition, player.position) <= 15f) {
			commingBack = false;
			mob.GetComponent<Animator> ().SetBool ("walking", true);
			m_Agent.destination = player.position;
		} else if (World.Instance.meshSurface != null) {
			m_Agent.destination = startPosition;
			commingBack = true;
		}
		if (World.Instance.meshSurface != null) {
			if (m_Agent.remainingDistance <= m_Agent.stoppingDistance && !commingBack) {
				mob.GetComponent<Animator> ().SetBool ("walking", false);
				transform.LookAt (player.position + new Vector3 (0, -0.5f, 0));
				if (mob.GetComponent<Animator> ().GetCurrentAnimatorStateInfo (0).IsName("idle") && Vector3.Distance(player.transform.position, mob.transform.position) <= 2) {
						mob.GetComponent<Animator> ().SetTrigger ("attacking");
					/*if (!mob.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).IsTag("wtf") && mob.GetComponent<Animator> ().GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f) {
						attacking = false;
						_mob.attack(character);
						Destroy(character.RemoveHeart ());

					}*/
				}
			} else if (m_Agent.remainingDistance <= m_Agent.stoppingDistance) {
				mob.GetComponent<Animator> ().SetBool ("walking", false);
			}
		}

		if (character.isCharacterDead ())
			SceneManager.LoadScene (SceneManager.GetActiveScene ().name);
		
		/*
		if (Vector3.Distance (transform.position, player.position) <= 1.5f) {
			transform.LookAt(player.position+ new Vector3(0,-0.5f,0));
			mob.GetComponent<Animator>().SetBool ("walking", false);
			mob.GetComponent<Animator> ().SetTrigger ("attacking");
		}
		else if (m_Agent.velocity != Vector3.zero) {
			mob.GetComponent<Animator>().SetBool ("walking", true);
		}
		else
			mob.GetComponent<Animator>().SetBool ("walking", false);
		}*/
	}
	public void AttackEnd() {
		_mob.attack(character);
		Destroy(character.RemoveHeart ());
	}
}
