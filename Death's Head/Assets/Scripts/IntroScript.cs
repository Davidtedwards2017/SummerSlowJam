using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;
using UnityEngine.SceneManagement;
using System;
//using UnityEditor;

public class IntroScript : MonoBehaviour {

    public static IntroScript Instance;

	//Lazy references to everything needed, as this is a singleton and it'll all be gone immediately afterwards
	[SerializeField] GameObject theKiller; //Killer Sprite
	[SerializeField] GameObject killTarget; //Target Sprite

	[SerializeField] GameObject fadeSpriteMask; //Fade (masks visible)
	[SerializeField] GameObject fadeSpriteFull; //Fade (Full black)

	[SerializeField] Camera mainCam; //Reference to the camera to teleport it later
	//[SerializeField] GameObject CamPoint; //Transform of which to teleport the camera.

	[SerializeField] GameObject maskPos;
	[SerializeField] Transform dropMask;

	[SerializeField] GameObject newPlayer;

	[SerializeField] AudioClip bellToll;
	[SerializeField] GameObject backSwap;
	[SerializeField] Sprite newBG;

	private AudioSource source;

    public Transform DeathParticlePrefab;

	public static bool gameStart; //Bool that will unlock player movement when the fade in is finished

    public SequenceActorController Child;
    public SequenceActorController Killer;
    public SequenceActorController Victim;

    public Transform SwingPosAnchor;
    public Transform OffscreenRightAnchor;
    public Transform VictimDeathAnchor;
    public Transform VictimWaitAnchor;

    public Transform FirstCamFocusAnchor;
    public Transform SecondCamFocusAnchor;

    [SerializeField] public Sprite JumpMask;
    [SerializeField] public Sprite DeathMask;

    public string SceneToLoad;

    //Values for alphas and fading
    float alphaMask = 0;
	float alphaFull = 0;
	float fadeSpeed = 0.2f;

	//Bools for checking the fading
	bool fadeOutStart;
	bool fadeInStart;

    public static int Replays = 0;

    bool SetupSequnceCompleted = false;
    bool PlayerSeenSequenceStarted = false;
    bool PlayerSeenSequenceCompleted = false;
    bool GiveUpsequenceStarted = false;

    void OnLevelWasLoaded(int level)
    {

    }

    // Use this for initialization
    void Start () 
	{
        Instance = this;
		fadeOutStart = false;
		fadeInStart = false;
		gameStart = false;
		source = this.gameObject.GetComponent<AudioSource> ();

        CameraFade.Instance.SetFade(false);
        Replays++;

        if (Replays == 1) //first playthrough
        {
            SetupFirstPlaythrough();
        }
    }

	void Update () 
	{
		//Press "action" button. This can change to whatever we're actually using
		if (SetupSequnceCompleted && Input.GetButtonDown("Jump")) 
		{
            StartCoroutine(KillSequence());
            SetupSequnceCompleted = false;
        }

        if (PlayerSeenSequenceStarted && PlayerSeenSequenceCompleted && /*DeathMask.Instance.Active &&*/ !GiveUpsequenceStarted)
        {
            StartCoroutine(GiveUpSequence());
        }



		//Fading out and in when Coroutine is done
		FadeOut ();
		FadeIn ();
	}

    public void SetupFirstPlaythrough()
    {
        //start half way through sequence (from prespective of child)
        Destroy(PlayerController.Instance.gameObject);
        CameraFollow.Instance.Target = SecondCamFocusAnchor;

        Child.SetVisable(true);
        Killer.SetVisable(true);
        Child.StartAnimation("idle", 0.2f, true);
        Killer.StartAnimation("kill_still", 0.5f, false);
        Victim.StartAnimation("kneel", 0.2f, false);
        Killer.transform.position = SwingPosAnchor.position;
        Victim.transform.position = VictimDeathAnchor.position;
        Victim.SwapMask(JumpMask);

        SetupSequnceCompleted = true;

    }

    public void StartPlayerSeenSequence()
    {
        StartCoroutine(PlayerSeenSequence());
    }

    IEnumerator PlayerSeenSequence()
    {
        Victim.SwapMask(DeathMask);
        PlayerSeenSequenceStarted = true;
        CameraFollow.Instance.Target = FirstCamFocusAnchor;
        CameraFollow.Instance.xSmooth = 2;

        //victim walks on screen
        Victim.MoveTo(VictimWaitAnchor, 2);
        Victim.StartAnimation("walk", 0.8f, true);
        PlayFootSteps(2, 0.7f);
        yield return new WaitForSeconds(2);

        //victim sees killer
        Victim.StartAnimation("idle", 0.5f, true);
        yield return new WaitForSeconds(1);
        ThrowDeathMask();
        Victim.SwapMask(JumpMask);
        yield return new WaitForSeconds(1);
        PlayerSeenSequenceCompleted = true;
    }

    IEnumerator GiveUpSequence()
    {
        Destroy(MaskUIController.Instance.gameObject);
        GiveUpsequenceStarted = true;
        Destroy(PlayerController.Instance.gameObject);
        Killer.SetVisable(true);
        CameraFollow.Instance.Target = FirstCamFocusAnchor;
        CameraFollow.Instance.xSmooth = 2;

        //victim kneels
        Victim.MoveTo(VictimDeathAnchor, 1.5f);
        Victim.StartAnimation("walk", 0.6f, true);
        PlayFootSteps(2, 0.8f);
        yield return new WaitForSeconds(2f);

        Victim.StartAnimation("kneel", 0.2f, true);
        yield return new WaitForSeconds(1.5f);

        Killer.StartAnimation("walk", 0.3f, true);
        Killer.MoveTo(SwingPosAnchor, 1);
        PlayFootSteps(1, 0.8f);

        //camera pans over to show kid
        CameraFollow.Instance.Target = SecondCamFocusAnchor;
        Child.SetVisable(true);

        yield return new WaitForSeconds(1);
        Killer.StartAnimation("idle", 0.3f, true);
        yield return new WaitForSeconds(1);
        Killer.StartAnimation("kill_still", 0.5f, false);

        SetupSequnceCompleted = true;
    }

	//Coroutine to set speed of sequence based on the speeds of the individual animations
	IEnumerator KillSequence ()
	{
        Killer.StartAnimation("kill_swing", 0.5f, false);
        yield return new WaitForSeconds(0.25f);

		source.PlayOneShot (bellToll);
        PlayDeathEffect(Victim.transform.position);
        backSwap.GetComponent<SpriteRenderer> ().sprite = newBG;
        DropMask();

        yield return new WaitForSeconds (0.1f);
        Victim.StartAnimation("death_kneel", 1.0f, false);

        //yield return new WaitForSeconds (1);
        Child.MoveTo(SwingPosAnchor, 4);
        Child.StartAnimation("run", 0.7f, true);
        yield return new WaitForSeconds(1);

        Killer.MoveTo(OffscreenRightAnchor, 5);
        Killer.StartAnimation("walk", 0.5f, true);
        PlayFootSteps(5, 0.8f);

        print ("The victim has been killed, slumps over, and dies");
		//Destroy (killTarget);

		fadeOutStart = true;
        yield return new WaitForSeconds(3f);

        Child.StartAnimation("kneel", 0.7f, false);
        yield return new WaitForSeconds(3f);

        FadeOut ();
		FadeIn ();
		ReloadLoadScene();
    }

    void DropMask()
    {
        Transform t = Instantiate(dropMask, maskPos.transform.position, Quaternion.identity) as Transform;
        t.GetComponent<SpriteRenderer>().flipX = true;
        t.GetComponent<Rigidbody2D>().AddForce(new Vector2(-20, 150));
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
            //mainCam.GetComponent<Transform>().position = CamPoint.GetComponent<Transform>().position; //Teleport camera to right position
            Destroy(fadeSpriteMask); //Cleanup the garbage
            fadeSpriteFull.GetComponent<SpriteRenderer>().color = new Color(0, 0, 0, alphaFull);
            alphaFull -= (fadeSpeed * Time.deltaTime);

            if (alphaFull <= 0)
            {
                //Start the game, destroy all of this garbage
                gameStart = true;
                Destroy(fadeSpriteFull);
                Destroy(this.gameObject);
            }


        }
    }

    public void ReloadLoadScene()
    {
        SceneManager.LoadScene(SceneToLoad);
    }

    void PlayDeathEffect(Vector3 position)
    {
        Instantiate(DeathParticlePrefab, position, Quaternion.identity);
        SoundController.Instance.PlaySound("Death");

    }

    public Transform deathMaskSpawnLocation;
    public Transform DeathMaskPickupPrefab;

    void ThrowDeathMask()
    {
        Transform t = Instantiate(DeathMaskPickupPrefab, deathMaskSpawnLocation.position, Quaternion.identity) as Transform;
        t.GetComponent<Rigidbody2D>().AddForce(new Vector2(-300, 210));
    }

    void PlayFootSteps(float duration, float timeBetweenSteps)
    {
        StartCoroutine(footSteps(duration, timeBetweenSteps));
    }

    IEnumerator footSteps(float duration, float timeBetweenSteps)
    {
        DateTime endTime = DateTime.UtcNow + TimeSpan.FromSeconds(duration);

        while(DateTime.UtcNow < endTime)
        {
            SoundController.Instance.PlaySound("footstep");
            yield return new WaitForSeconds(timeBetweenSteps);
        }
    }
}
