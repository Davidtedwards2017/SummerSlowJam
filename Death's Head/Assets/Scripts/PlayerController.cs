using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PlayerController : MonoBehaviour {

	//Set Dictionary to act as a finite State Machine
	private Dictionary <PlayerStates, Action> fsm = new Dictionary<PlayerStates, Action>();

	//Set enum to hold state of state machine
	public enum PlayerStates
	{
		IDLE,
		MOVE,
		JUMP,
		DEATH,
		PUSH
	}
		

	//Float values for speed and jump force
	public float p_Speed;
	public float p_JumpForce;

	//Value to store axis input
	float horizontal;

	//Boolean values to determine what the player is doing
	bool p_Move;
	bool p_Jump;
	bool p_onGround;

	//Bool value to determine player's facing
	bool p_facingRight;

	//References to player components
	private Rigidbody2D p_Rigidbody;
	private Transform p_groundCheck;

	//Ground radius value to check if ground is under player
	const float p_groundRadius = 0.2f;

	//LayerMask to only detect ground layer
	[SerializeField] LayerMask p_checkGround;

	//Value to store current player state
	private PlayerStates p_State;

	// Use this for initialization
	void Awake () 
	{
		//On awake, add all states to the dictionary
		fsm.Add (PlayerStates.IDLE, IdleState);
		fsm.Add (PlayerStates.JUMP, JumpState);
		fsm.Add (PlayerStates.DEATH, DeathState);
		fsm.Add (PlayerStates.MOVE, MoveState);
		fsm.Add (PlayerStates.PUSH, PushState);
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

		//Set state to IDLE to start
		SetState (PlayerStates.IDLE);
	}
	
	// Update is called once per frame
	void FixedUpdate () 
	{
		InputManager (); //Set Inputs
		CheckGround (); //Check if the player is on ground
		MoveManager (horizontal); //Move the player

		fsm [p_State].Invoke (); //Enact the current state, change states when necessary
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
			p_Rigidbody.velocity = Vector2.zero;
		}

		//On ground and jump, add the jumpforce
		if (p_onGround && p_Jump) 
		{
			p_onGround = false;
			p_Move = false;
			p_Rigidbody.velocity = Vector2.zero;
			p_Rigidbody.AddForce (new Vector2 (0, p_JumpForce), ForceMode2D.Impulse);
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
		horizontal = Input.GetAxisRaw ("Horizontal");
		p_Jump = Input.GetButtonDown ("Jump");
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
		}
	}

	void Flip ()
	{
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
		if (p_Move)
			SetState (PlayerStates.MOVE);
		else if (p_Jump) 
		{
			SetState (PlayerStates.JUMP);
		}
	}

	void MoveState ()
	{
		if (!p_Move)
			SetState (PlayerStates.IDLE);
		else if (p_Jump)
			SetState (PlayerStates.JUMP);
	}

	void JumpState ()
	{
		if (p_onGround)
			SetState (PlayerStates.IDLE);
	}


	//To be added
	void DeathState ()
	{
		return;
	}

	void PushState ()
	{
		return;
	}
		
}
