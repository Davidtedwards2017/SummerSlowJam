using UnityEngine;
using System.Collections;
using System;

public class JumpMask : Mask
{
    protected override void Activated()
    {
        PlayerController.Instance.CanJump = true;
    }

    protected override void Deactivated()
    {
        PlayerController.Instance.CanJump = false;
    }
}
