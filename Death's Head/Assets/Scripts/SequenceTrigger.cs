using UnityEngine;
using System.Collections;

public class SequenceTrigger : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag.Equals("Player"))
        {
            Destroy(PlayerController.Instance.gameObject);
            IntroScript.Instance.StartSetupSequence();
        }
    }
}
