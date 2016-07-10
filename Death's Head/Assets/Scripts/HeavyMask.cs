using UnityEngine;
using System.Collections;
using System;

public class HeavyMask : Mask {
    
    public Transform GrabSocket;
    public float grabDistance = 5;
    public float PushPullForce = 10;
    
    public MovingBlock GrabbedBlock;

    // Update is called once per frame
    void Update()
    {
        PlayerController.Instance.CanFlip = true;
        if (Input.GetKey(KeyCode.R))
        {
            var grabbedBlock = GetNearbyBlock();
            if (grabbedBlock != null)
            {
                StartGrab(grabbedBlock);
            }

        }
        else if (GrabbedBlock != null)
        {
            StopGrab();
        }

        if (GrabbedBlock != null)
        {
            Vector3 dir = (GrabSocket.transform.position - GrabbedBlock.transform.position).normalized;
            GrabbedBlock.ApplyForce(dir * PushPullForce, GrabbedBlock.transform.position);
        }

    }

    public void StartGrab(MovingBlock block)
    {
        PlayerController.Instance.CanFlip = false;
        PlayerController.Instance.p_Speed = PlayerController.Instance.p_GrabRunSpeed;
        GrabbedBlock = block;
    }

    public void StopGrab()
    {
        PlayerController.Instance.CanFlip = true;
        PlayerController.Instance.p_Speed = PlayerController.Instance.p_NormalRunSpeed;
        GrabbedBlock = null;
    }
        
    public MovingBlock GetNearbyBlock()
    {
        var blocks = GameObject.FindObjectsOfType<MovingBlock>();
        foreach(var block in blocks)
        {
            
            if(Vector3.Distance(GrabSocket.position, block.transform.position) <= grabDistance)
            {
                return block;
            }
            
        }

        return null;
    }
   

    protected override void Activated()
    {
        MovingBlock.Moveable = true;
    }

    protected override void Deactivated()
    {
        MovingBlock.Moveable = false;
    }
    
}
