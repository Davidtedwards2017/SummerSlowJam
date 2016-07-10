using UnityEngine;
using System.Collections;

public abstract class Mask : MonoBehaviour {

    public bool Active = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
    
    public void SetActive(bool active)
    {
        if(Active != active)
        {
            Active = active;

            if(Active)
            {
                Activated();
            }
            else
            {
                Deactivated();
            }
        }
    }

    protected abstract void Activated();
    protected abstract void Deactivated();
}
