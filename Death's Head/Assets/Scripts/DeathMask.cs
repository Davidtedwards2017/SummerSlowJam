
public class DeathMask : Mask {

    protected override void Activated()
    {
        PlayerController.Instance.SetScytheActive(true);
    }

    protected override void Deactivated()
    {
        PlayerController.Instance.SetScytheActive(false);
    }


}
