using UnityEngine;
using System.Collections;
using Spine.Unity;
using Spine.Unity.Modules;

public abstract class Mask : MonoBehaviour {

    public bool Active = false;

    public Sprite MaskSprite;
    public Transform GraphicPrefab;
    public Transform Graphic;

    private SkeletonAnimation m_SkeletonAnimation;

	// Use this for initialization
	void Awake () {
        m_SkeletonAnimation = PlayerController.Instance.GetComponentInChildren<SkeletonAnimation>();
    }
	
	// Update is called once per frame
	protected virtual void Update () {
        if(Graphic != null)
        {
            Transform socket = PlayerController.Instance.MaskSocket;

            Graphic.transform.position = socket.position;
            Graphic.transform.rotation = socket.rotation;
        }
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
                Graphic = null;
            }
        }
    }

    public void SetGraphicVisable(bool visable)
    {
        if (visable)
        {
            m_SkeletonAnimation.skeleton.AttachUnitySprite("head2", MaskSprite);
        }
    }

   

    public void SetMaskGraphic()
    {
        //TODO: change this later to work with spine
        Transform socket = PlayerController.Instance.MaskSocket;
        Graphic = Instantiate(GraphicPrefab, socket.position, socket.rotation) as Transform;
    }

    protected abstract void Activated();
    protected abstract void Deactivated();
}
