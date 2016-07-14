using UnityEngine;
using System.Collections;

public class LightFlicker : MonoBehaviour {

	SpriteRenderer mySprite;
	float alphaLev;

	void Awake ()
	{
		mySprite = this.gameObject.GetComponent<SpriteRenderer> ();
	}


	// Use this for initialization
	void Start () 
	{
		StartCoroutine ("Flicker");

	}

	IEnumerator Flicker ()
	{
		while (true) {
			yield return new WaitForSeconds (.05f);
			alphaLev = 1.0f * Random.Range (.75f, 1.0f);
			mySprite.color = new Color (1, 1, 1, alphaLev);
		} 
	}
}
