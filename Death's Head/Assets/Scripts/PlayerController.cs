using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Spine.Unity;
using Spine.Unity.Modules;

public class PlayerController : MonoBehaviour {

	//Set Dictionary to act as a finite State Machine
	private Dictionary <PlayerStates, Action> fsm = new Dictionary<PlayerStates, Action>();

    public static PlayerController Instance;

    private const string IdleAnimationName = "idle";
    private const string RunAnimationName = "run";
    private const string PushAnimationName = "Push";

    private const string GrabhAnimationName = "grab idle";
    private const string PullAnimationName = "pull";
    private const string DeathAnimationName = "death";
    private const string JumpAnimationName = "jump";

    public SkeletonAnimation m_Animator;

	//Set enum to hold state of state machine
	public enum PlayerStates
	{
		IDLE,
		MOVE,
		JUMP,
		DEATH,
		PUSH,
        GRAB,
        PULL
	}

    public Transform MaskSocket;
    public Transform GrabSocket;

	//Float values for speed and jump force
	public float p_Speed;
    public float p_NormalRunSpeed;
    public float p_GrabRunSpeed;
	public float p_JumpForce;

	//Value to store axis input
	float horizontal;

	//Boolean values to determine what the player is doing
	bool p_Move;
	bool p_Jump;
	bool p_onGround;
    bool p_Toggle;

	//Bool value to determine player's facing
	bool p_facingRight;

	//References to player components
	private Rigidbody2D p_Rigidbody;
	private Transform p_groundCheck;

	//Ground radius value to check if ground is under player
	const float p_groundRadius = 0.2f;

	//LayerMask to only detect ground layer
	[SerializeField] LayerMask p_checkGround;

    //current equiped mask
    public Mask CurrentMask;

	[SerializeField] GameObject MaskGO;
	public GameObject[] maskUI;

    public bool CanFlip = true;
    public bool CanJump = false;
    public bool CanReceiveInput = true;
    
    //input cooldown times
    private float m_CurrentToggleCooldown;
    public float MaskToggleCooldownTime = 0.1f;

    private float m_StepSoundCooldown;
    public float RunStepSoundCooldown = 0.1f;
    public float DragStepSoundCooldown = 0.2f;

    public List<Mask> AquiredMasks
    {
        get { return GetComponentsInChildren<Mask>().ToList(); }
    }


	//Value to store current player state
	public PlayerStates p_State;

    private Spine.Slot scytheSlot;
    public Spine.Attachment scytheAttachment;
    public Sprite blank;

	[SerializeField] GameObject deathPoof;
    
	// Use this for initialization
	void Awake () 
	{
		//On awake, add all states to the dictionary
		fsm.Add (PlayerStates.IDLE, IdleState);
		fsm.Add (PlayerStates.JUMP, JumpState);
		fsm.Add (PlayerStates.DEATH, DeathState);
		fsm.Add (PlayerStates.MOVE, MoveState);
		fsm.Add (PlayerStates.PUSH, PushState);
        fsm.Add (PlayerStates.GRAB, GrabState);
        fsm.Add (PlayerStates.PULL, PullState);

        Instance = this;
	
	}



	void Start ()
	{
        //Reset all values to default at start
        p_Move = false; 
		p_onGround = true;
		p_facingRight = true;

		//Set references
		p_Rigidbody = this.gameObject.GetComponent<Rigidbody2D> ();
		p_groundCheck = transform.Find ("GroundCheck");
        m_Animator = GetComponentInChildren<SkeletonAnimation>();
		maskUI = GameObject.FindGameObjectsWithTag ("Masks");
		MaskGO.SetActive (false);

        scytheSlot = m_Animator.skeleton.slots.Items.FirstOrDefault(slot => slot.ToString().Equals("scythe"));
        scytheAttachment = scytheSlot.Attachment;
        SetScytheActive(false);


        //Set state to IDLE to start
        SetState (PlayerStates.IDLE);

    }
	
    public void SetScytheActive(bool active)
    {
        if(active)
        {
            scytheSlot.Attachment = scytheAttachment;
        }
        else
        {
            m_Animator.skeleton.AttachUnitySprite("scythe", blank, "Unlit/Transparent");
        }
    }

    public void AquireMask(Transform prefab)
    {
        Transform maskTransform = Instantiate(prefab) as Transform;
        maskTransform.SetParent(transform.FindChild("Masks"));

        EquipMask(maskTransform.GetComponent<Mask>());
    }

    public void EquipNextMask()
    {
        //get index of current mask
        var masks = AquiredMasks;
        int index = masks.IndexOf(CurrentMask);

        //cycle mask index
        index++;
        if(index >= masks.Count)
        {
            index = 0;
        }

        EquipMask(masks[index]);
    }

    public void EquipMask(Mask newMask)
    {
        //de equip current mask
		if (CurrentMask != null && AquiredMasks.Count <= 2) 
		{
			CurrentMask.SetActive (false);
			maskUI [0].GetComponent<Image> ().sprite = CurrentMask.MaskSprite;
		} 

		else if (CurrentMask != null && AquiredMasks.Count >= 2) 
		{
			CurrentMask.SetActive (false);
		}

        if(CurrentMask != newMask)
        {
            SoundController.Instance.PlaySound("maskequip");
        }

        // equip next mask
        CurrentMask = newMask;
        CurrentMask.SetActive(true);
		MaskGO.SetActive (true);
		maskUI [1].GetComponent<Image> ().sprite = CurrentMask.MaskSprite;
   }

	// Update is called once per frame
	void FixedUpdate () 
	{
		InputManager (); //Set Inputs
		CheckGround (); //Check if the player is on ground
		MoveManager (horizontal); //Move the player

		fsm [p_State].Invoke (); //Enact the current state, change states when necessary
	}

    void Update()
    {
        m_StepSoundCooldown -= Time.deltaTime;

        if(m_CurrentToggleCooldown > 0)
        {
            m_CurrentToggleCooldown -= Time.deltaTime;
        }
		else if(p_Toggle && AquiredMasks.Count >= 2)
        {
            EquipNextMask();
            m_CurrentToggleCooldown = MaskToggleCooldownTime;
        }
    }
		
	public void SetState (PlayerStates nextState)
	{
		if (p_State != nextState)
			p_State = nextState;
	}

	/// HELPER FUNCTIONS ///

	//Function to manage all player movement, 
	void MoveManager (float horizontal)
	{
		//Add movement to actor if player is inputting movement
		p_Rigidbody.velocity = new Vector2 (horizontal * p_Speed, p_Rigidbody.velocity.y);

		//If you're moving, set move to true
		if (horizontal != 0f)
			p_Move = true;

		//No movement and on ground, set to false and halt velocity
		else if (horizontal == 0 && p_onGround) 
		{
			p_Move = false;
			//p_Rigidbody.velocity = Vector2.zero;
		}

		//On ground and jump, add the jumpforce
		if (p_onGround && p_Jump && CanJump) 
		{
			p_onGround = false;
			p_Move = false;
			//p_Rigidbody.velocity = Vector2.zero;
			p_Rigidbody.AddForce (new Vector2 ((horizontal * p_Speed) , p_JumpForce), ForceMode2D.Impulse);
            SoundController.Instance.PlaySound("jump");
		}

		//If you're facing right, look right. If facing left, look left
		if (horizontal > 0 && !p_facingRight || horizontal < 0 && p_facingRight) 
		{
			Flip ();
		}
	}

	//Function to manage buttons
	void InputManager ()
	{
        if(!CanReceiveInput)
        {
            horizontal = 0;
            p_Jump = false;
            return;
        }

		horizontal = Input.GetAxisRaw ("Horizontal");
		p_Jump = Input.GetButtonDown ("Jump");


        p_Toggle = Input.GetKey(KeyCode.W);

		if (Input.GetKeyDown (KeyCode.F1)) 
		{
			SceneManager.LoadScene ("Main");
		}
	}

	//Function to determine whether or not the player is on the ground
	void CheckGround ()
	{
		//Set an array of colliders for every collider around the groundcheck
		Collider2D[] colliders = Physics2D.OverlapCircleAll (p_groundCheck.position, p_groundRadius, p_checkGround);

		//If any of those colliders is "Ground" layered, you're on the ground.
		for (int i = 0; i < colliders.Length; i++) 
		{
			if (colliders [i].gameObject != gameObject)
				p_onGround = true;
			else
				p_onGround = false;
		}
	}

	void Flip ()
	{
        if(!CanFlip)
        {
            return;
        }

		//If you're flipping, flip the bool
		p_facingRight = !p_facingRight;

		//Flip scale to mirror sprite
		Vector3 p_Scale = transform.localScale;
		p_Scale.x *= -1;
		transform.localScale = p_Scale;
	}

	/// STATE FUNCTIONS ///

	//Each state function operates and switches based on the current bool/state
	//This can be used to transition sounds, animations, etc.
	//Also prevents silly things like jumping when you're dead.

	void IdleState ()
	{
        m_Animator.AnimationName = IdleAnimationName;
        m_Animator.loop = true;

        if (p_Move)
			SetState (PlayerStates.MOVE);
		else if (p_Jump && CanJump) 
		{
			SetState (PlayerStates.JUMP);
		}
	}

	void MoveState ()
	{
        if(m_StepSoundCooldown <= 0)
        {
            SoundController.Instance.PlaySound("footstep");
            m_StepSoundCooldown = RunStepSoundCooldown;
        }

        m_Animator.AnimationName = RunAnimationName;
        m_Animator.loop = true;

        if (!p_Move)
			SetState (PlayerStates.IDLE);
		else if (!p_onGround || (p_Jump && CanJump))
			SetState (PlayerStates.JUMP);
	}

	void JumpState ()
	{
        if (p_onGround)
        {
            SoundController.Instance.PlaySound("land");
            SetState(PlayerStates.IDLE);
        }

        m_Animator.AnimationName = JumpAnimationName;
        m_Animator.loop = true;
    }

   
    void SpawnAtPrevCheckpoint()
    {
        var checkpoint = Checkpoint.PrevCheckPoint;
        if(checkpoint != null)
        {
            checkpoint.SpwanAtCheckpoint();
        }
    }

	//To be added
	void DeathState ()
	{
        return;
	}

    public void Death()
    {
        SoundController.Instance.PlaySound("death");
        m_Animator.loop = false;
        m_Animator.AnimationName = DeathAnimationName;
        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        SetState(PlayerStates.DEATH);
        CanReceiveInput = false;
		Instantiate (deathPoof, this.gameObject.transform.position, deathPoof.transform.rotation);
        yield return new WaitForSeconds(1f);
        CameraFade.Instance.SetFade(true);
        yield return new WaitForSeconds(0.2f);

        SpawnAtPrevCheckpoint();

        CameraFade.Instance.SetFade(false);
        yield return new WaitForSeconds(0.2f);

        SetState(PlayerStates.IDLE);
        CanReceiveInput = true;
    }


    public void StartGrab()
    {
        if (m_StepSoundCooldown <= 0)
        {
            SoundController.Instance.PlaySound("footstep");
            m_StepSoundCooldown = DragStepSoundCooldown;
        }

        SetState(PlayerStates.GRAB);
        CanFlip = false;
        p_Speed = p_GrabRunSpeed;
    }

    public void EndGrab()
    {
        SetState(PlayerStates.IDLE);
        CanFlip = true;
        p_Speed = p_NormalRunSpeed;
    }

    void GrabState()
    {
        m_Animator.AnimationName = GrabhAnimationName;
        m_Animator.loop = true;

        
        if(p_Move && (!p_facingRight && horizontal > 0) || (horizontal < 0 && p_facingRight))
        {
            SetState(PlayerStates.PULL);
        }
        else if (p_Move)
        {
            SetState(PlayerStates.PUSH);
        }
        
    }

    public void PullState()
    {
        if (m_StepSoundCooldown <= 0)
        {
            SoundController.Instance.PlaySound("footstep");
            m_StepSoundCooldown = DragStepSoundCooldown;
        }

        m_Animator.AnimationName = PullAnimationName;
        m_Animator.loop = true;

        if (!p_Move)
        {
            SetState(PlayerStates.GRAB);
        }
        else if(IsPushing())
        {
            SetState(PlayerStates.PUSH);
        }

    }

	void PushState ()
	{
        if (m_StepSoundCooldown <= 0)
        {
            SoundController.Instance.PlaySound("footstep");
            m_StepSoundCooldown = DragStepSoundCooldown;
        }

        m_Animator.AnimationName = PushAnimationName;
        m_Animator.loop = true;

        if (!p_Move)
        {
            SetState(PlayerStates.GRAB);
        }
        else if(IsPulling())
        {
            SetState(PlayerStates.PULL);
        }
    }

    private bool IsPulling()
    {
        return(p_Move && (!p_facingRight && horizontal > 0) || (horizontal < 0 && p_facingRight));
    }

    private bool IsPushing()
    {
        return (p_Move && (!p_facingRight && horizontal < 0) || (horizontal > 0 && p_facingRight));
    }
		
}
