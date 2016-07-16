using UnityEngine;
using System.Collections;

public class BGChange : MonoBehaviour {


	[SerializeField] GameObject oldBG;
	[SerializeField] Sprite newSprite;

	[SerializeField] AudioClip myClip;
	private AudioSource source;

	// Use this for initialization
	void Start () 
	{
		source = this.gameObject.GetComponent<AudioSource> ();
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}

	void OnTriggerEnter2D (Collider2D col)
	{
		if (col.tag == "Player") 
		{
			oldBG.GetComponent<SpriteRenderer> ().sprite = newSprite;
			source.PlayOneShot (myClip);
			this.gameObject.GetComponent<BoxCollider2D> ().enabled = false;
		}
	}
}
