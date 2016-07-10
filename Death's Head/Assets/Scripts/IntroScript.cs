using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour {

	[SerializeField] GameObject theKiller;
	[SerializeField] GameObject killTarget;
	[SerializeField] GameObject fadeSpriteMask;
	[SerializeField] GameObject fadeSpriteFull;

	[SerializeField] Camera mainCam;
	[SerializeField] GameObject CamPoint;
	public static bool gameStart;

	float alphaMask = 0;
	float alphaFull = 0;
	float fadeSpeed = 0.2f;

	bool fadeOutStart;
	bool fadeInStart;


	// Use this for initialization
	void Start () 
	{
		fadeOutStart = false;
		fadeInStart = false;
		gameStart = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetKeyDown (KeyCode.F)) 
		{
			StartCoroutine ("KillSequence");
		}

		FadeOut ();
		FadeIn ();
	}

	IEnumerator KillSequence ()
	{
		print ("The killer swings");
		yield return new WaitForSeconds (3);
		print ("The victim has been killed, slumps over, and dies");
		yield return new WaitForSeconds (3);
		Destroy (killTarget);
		fadeOutStart = true;
		yield break;
	}

	void FadeOut ()
	{

		if (fadeOutStart) 
		{
			fadeSpriteMask.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, alphaMask);
			alphaMask += (fadeSpeed * Time.deltaTime);

			if (alphaMask >= 1) 
			{
				alphaMask = 1;
				fadeSpriteFull.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, alphaFull);
				alphaFull += (fadeSpeed * Time.deltaTime);

				if (alphaFull >= 1) 
				{
					Destroy (fadeSpriteMask);
					fadeOutStart = false;
					fadeInStart = true;
				}
			}
		}
	}

	void FadeIn()
	{
		if (fadeInStart) 
		{
			mainCam.GetComponent<Transform> ().position = CamPoint.GetComponent<Transform> ().position;
			Destroy (fadeSpriteMask);
			fadeSpriteFull.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, alphaFull);
			alphaFull -= (fadeSpeed * Time.deltaTime);

			if (alphaFull <= 0) 
			{
				gameStart = true;
				Destroy (fadeSpriteFull);
				Destroy (this.gameObject);
			}
		}

	}
}
