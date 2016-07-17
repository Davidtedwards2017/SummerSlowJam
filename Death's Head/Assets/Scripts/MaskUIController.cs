using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MaskUIController : MonoBehaviour {

    private Image[] MaskUISpots;
    public float EquipedScale;
    public float UnequipedScale;

    public static MaskUIController Instance;

    void Awake()
    {
        Instance = this;
    }
    // Use this for initialization
    void Start () {
        MaskUISpots = GetComponentsInChildren<Image>();
	}
	
	// Update is called once per frame
	void Update () {

        if(PlayerController.Instance == null)
        {
            return;
        }

        var masks = PlayerController.Instance.AquiredMasks;

        for(int i = 0; i < MaskUISpots.Length; i ++)
        {
            if(i >= masks.Count)
            {
                MaskUISpots[i].enabled = false;
                continue;
            }

            var mask = masks[i];
            MaskUISpots[i].enabled = true;
            MaskUISpots[i].sprite = mask.UISprite;
            MaskUISpots[i].transform.localScale = Vector3.one * ((mask.Active) ? EquipedScale : UnequipedScale);

        }

	}

    public void SetUiVisable(bool visable)
    {
        for (int i = 0; i < MaskUISpots.Length; i++)
        {
            MaskUISpots[i].enabled = visable;
        }
    }

}
