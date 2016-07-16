using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public abstract class Mask : MonoBehaviour {

    public bool Active = false;

    public Sprite MaskSprite;
	
	// Update is called once per frame
	protected virtual void Update () {

    }
    
    public void SetActive(bool active)
    {
        if(Active != active)
        {
            Active = active;

            if(Active)
            {
                Activated();
                SetGraphicVisable(true);
            }
            else
            {
                Deactivated();
                SetGraphicVisable(false);
            }
        }
    }

    public void SetGraphicVisable(bool visable)
    {
        if (visable)
        {
            PlayerController.Instance.m_Animator.skeleton.AttachUnitySprite("Mask_blank", MaskSprite);
            //PlayerController.Instance.m_Animator.skeleton.AttachUnitySprite("Mask_blank", MaskSprite, "Unlit/Transparent");
        }
    }

    protected abstract void Activated();
    protected abstract void Deactivated();
}
