using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : MonoBehaviour {
	public ParticleSystem particle;
	public List<AudioClip> GrassAudio;
	public List<AudioClip> StoneAudio;
	public List<AudioClip> WoodAudio;
	private static ResourcesManager instance = null;
	
	public static ResourcesManager Instance {
		get {
			return instance;
		}
	}

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		particle = Instantiate (particle);
		//particles = GetComponent<ParticleSystem> ();
		//ParticleSystemRenderer renderer = particles.GetComponent<ParticleSystemRenderer>();
		//CreateCube(ItemTexture.Bedrock);
		//renderer.material =
	}

	public ParticleSystem GetParticle() {
		return particle;
	}
}
