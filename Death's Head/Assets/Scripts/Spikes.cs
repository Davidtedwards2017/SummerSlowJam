using UnityEngine;
using System.Collections;

public class Spikes : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
          //PlayerController.Instance
          Debug.Log("player entered spikes");
            PlayerController.Instance.Death();
        }

        //if(other.)
    }
}
