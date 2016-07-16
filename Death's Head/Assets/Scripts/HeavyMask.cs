using UnityEngine;
using System.Collections;
using System;

public class HeavyMask : Mask {
    
    public Transform GrabSocket;
    public float grabDistance = 5;
    public float PushPullForce = 10;
    
    public MovingBlock GrabbedBlock;

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
            
        if(!Active)
        {
            return;
        }

        bool grabInput = Input.GetButton("Jump");
        if (grabInput && GrabbedBlock == null)
        {
            var grabbedBlock = GetNearbyBlock();
            if (grabbedBlock != null)
            {
                StartGrab(grabbedBlock);
            }

        }
        else if(!grabInput && GrabbedBlock != null)
        {
            StopGrab();
        }

        MovingBlock.Moveable = (GrabbedBlock != null);

        if (GrabbedBlock != null)
        {
            Vector3 dir = (PlayerController.Instance.GrabSocket.position - GrabbedBlock.transform.position).normalized;
            GrabbedBlock.ApplyForce(dir * PushPullForce, GrabbedBlock.transform.position);
        }
    }

    public void StartGrab(MovingBlock block)
    {
        PlayerController.Instance.StartGrab();
        GrabbedBlock = block;
    }

    public void StopGrab()
    {
        PlayerController.Instance.EndGrab();
        GrabbedBlock = null;
    }
        
    public MovingBlock GetNearbyBlock()
    {
        var blocks = GameObject.FindObjectsOfType<MovingBlock>();
        foreach(var block in blocks)
        {
            
            if(Vector3.Distance(PlayerController.Instance.GrabSocket.position, block.transform.position) <= grabDistance)
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
