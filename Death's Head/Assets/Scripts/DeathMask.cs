
using System.Collections.Generic;

public class DeathMask : Mask {

    public static DeathMask Instance;
    void Awake()
    {
        Instance = this;
    }

    protected override void Activated()
    {
        //DropOtherMasks();
        PlayerController.Instance.SetScytheActive(true);
    }

    protected override void Deactivated()
    {
        PlayerController.Instance.SetScytheActive(false);
    }

    private void DropOtherMasks()
    {
        List<Mask> masks = PlayerController.Instance.AquiredMasks;

        foreach(var mask in masks)
        {
            if(mask != this)
            {
                Destroy(mask.gameObject);
            }
        }
    }

}
