using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UVScroller : MonoBehaviour {
	private Vector2 uvSpeed = new Vector2( 0.0f, 0.01f );
	private Vector2 uvOffset = Vector2.zero;

	void LateUpdate() {
		uvOffset += ( uvSpeed * Time.deltaTime );

		//ensure we don't scroll the texture too far 
		if(uvOffset.x > 0.0625f) uvOffset = new Vector2(0,uvOffset.y);
		if(uvOffset.y > 0.0625f) uvOffset = new Vector2(uvOffset.x,0);

		this.GetComponent<Renderer>().materials[0].
		SetTextureOffset("_MainTex", uvOffset);
	}
}
