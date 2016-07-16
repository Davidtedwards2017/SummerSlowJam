using UnityEngine;
using System.Collections;

public class MoveBoundary : MonoBehaviour {

	public GameObject PlayerRef;
	public float yBound;

	// Use this for initialization
	void Start () 
	{
		PlayerRef = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () 
	{
        if (PlayerRef != null)
        {
            this.gameObject.transform.position = new Vector2(transform.position.x, PlayerRef.GetComponent<Transform>().position.y + yBound);
        }
	}
}
