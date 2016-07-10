using UnityEngine;
using System.Collections;

public abstract class Mask : MonoBehaviour {

    public bool Active = false;

    public Transform GraphicPrefab;
    public Transform Graphic;

	// Use this for initialization
	void Start () {
        
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
                SetMaskGraphic();
            }
            else
            {
                Deactivated();
                Destroy(Graphic.gameObject);
                Graphic = null;
            }
        }
    }

    public void SetGraphicVisable(bool visable)
    {
        
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
