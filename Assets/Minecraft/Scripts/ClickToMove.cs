using UnityEngine;
using UnityEngine.AI;

// Use physics raycast hit from mouse click to set agent destination
[RequireComponent(typeof(NavMeshAgent))]
public class ClickToMove : MonoBehaviour
{
	public Transform player;
	public Transform mob;
	NavMeshAgent m_Agent;
	RaycastHit m_HitInfo = new RaycastHit();
	Vector3 startPosition;
	bool commingBack = true;
	void Start() {
		startPosition = transform.position;
		m_Agent = GetComponent<NavMeshAgent>();
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

		if (m_Agent.remainingDistance <= m_Agent.stoppingDistance && !commingBack) {
			mob.GetComponent<Animator> ().SetBool ("walking", false);
			transform.LookAt (player.position + new Vector3 (0, -0.5f, 0));
			mob.GetComponent<Animator> ().SetTrigger ("attacking");
		} else if (m_Agent.remainingDistance <= m_Agent.stoppingDistance) {
			mob.GetComponent<Animator> ().SetBool ("walking", false);
		}
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
}
