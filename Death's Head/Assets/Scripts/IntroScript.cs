using UnityEngine;
using System.Collections;

public class IntroScript : MonoBehaviour {

	//Lazy references to everything needed, as this is a singleton and it'll all be gone immediately afterwards
	[SerializeField] GameObject theKiller; //Killer Sprite
	[SerializeField] GameObject killTarget; //Target Sprite

	[SerializeField] GameObject fadeSpriteMask; //Fade (masks visible)
	[SerializeField] GameObject fadeSpriteFull; //Fade (Full black)

	[SerializeField] Camera mainCam; //Reference to the camera to teleport it later
	[SerializeField] GameObject CamPoint; //Transform of which to teleport the camera.

	public static bool gameStart; //Bool that will unlock player movement when the fade in is finished

    public SequenceActorController Child;
    public SequenceActorController Killer;
    public SequenceActorController Victim;

    public Transform SwingPosAnchor;
    public Transform OffscreenRightAnchor;

    //Values for alphas and fading
    float alphaMask = 0;
	float alphaFull = 0;
	float fadeSpeed = 0.2f;

	//Bools for checking the fading
	bool fadeOutStart;
	bool fadeInStart;


	// Use this for initialization
	void Start () 
	{
		fadeOutStart = false;
		fadeInStart = false;
		gameStart = false;
	}

	void Update () 
	{
		//Press "action" button. This can change to whatever we're actually using
		if (Input.GetKeyDown (KeyCode.F)) 
		{
			StartCoroutine ("KillSequence");
		}

		//Fading out and in when Coroutine is done
		FadeOut ();
		FadeIn ();
	}

	//Coroutine to set speed of sequence based on the speeds of the individual animations
	IEnumerator KillSequence ()
	{
		print ("The killer swings");

        Killer.StartAnimation("run", 0.3f, true);
        Killer.MoveTo(SwingPosAnchor, 1);
        yield return new WaitForSeconds(1);
        Killer.StartAnimation("idle", 0.3f, true);
        yield return new WaitForSeconds(1);
        Killer.StartAnimation("kill", 0.5f, false);

        yield return new WaitForSeconds(1.3f);
        Victim.StartAnimation("death", 1.0f, false);

        //yield return new WaitForSeconds (1);
        Child.MoveTo(SwingPosAnchor, 6);
        Child.StartAnimation("run", 0.7f, true);
        yield return new WaitForSeconds(1);

        Killer.MoveTo(OffscreenRightAnchor, 5);
        Killer.StartAnimation("run", 0.5f, true);
        
		print ("The victim has been killed, slumps over, and dies");
		Destroy (killTarget);
		fadeOutStart = true;
		yield break;
	}

	//Function for fading out
	void FadeOut ()
	{

		if (fadeOutStart) 
		{
			fadeSpriteMask.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, alphaMask); //Set the new alpha
			alphaMask += (fadeSpeed * Time.deltaTime);

			if (alphaMask >= 1) 
			{
				alphaMask = 1;
				fadeSpriteFull.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, alphaFull);
				alphaFull += (fadeSpeed * Time.deltaTime);

				if (alphaFull >= 1) 
				{
					Destroy (fadeSpriteMask); //Cleanup the garbage
					fadeOutStart = false; //Reverse the bools
					fadeInStart = true;
				}
			}
		}
	}


	//Function for fading back in
	void FadeIn()
	{
		if (fadeInStart) 
		{
			mainCam.GetComponent<Transform> ().position = CamPoint.GetComponent<Transform> ().position; //Teleport camera to right position
			Destroy (fadeSpriteMask); //Cleanup the garbage
			fadeSpriteFull.GetComponent<SpriteRenderer> ().color = new Color (0, 0, 0, alphaFull); 
			alphaFull -= (fadeSpeed * Time.deltaTime);

			if (alphaFull <= 0) 
			{
				//Start the game, destroy all of this garbage
				gameStart = true;
				Destroy (fadeSpriteFull);
				Destroy (this.gameObject);
			}
		}

	}
}
