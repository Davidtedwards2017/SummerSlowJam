using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingBlock : MonoBehaviour {


    //this should be static to effect all moveable blocks
    public static bool Moveable = false;

    private Rigidbody2D rigidBody;
    
    void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
    public void ApplyForce(Vector2 force, Vector2 pos)
    {
        rigidBody.AddForceAtPosition(force, pos);
    }


}
