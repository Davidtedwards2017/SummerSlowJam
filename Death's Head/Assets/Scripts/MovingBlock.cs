using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingBlock : MonoBehaviour {


    //this should be static to effect all moveable blocks
    public static bool Moveable = false;

    public float MinSpeedForSound = 1;

    private Vector2 lastVelocity;
    private Rigidbody2D rigidBody;

    private AudioSource audioSource;
    
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        audioSource = GetComponent<AudioSource>();
    }
	
	// Update is called once per frame
    public void ApplyForce(Vector2 force, Vector2 pos)
    {
        rigidBody.AddForceAtPosition(force, pos);
    }

    public void Update()
    {
        if (rigidBody.velocity.magnitude > MinSpeedForSound)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }

        }
        else if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }
    }


}
