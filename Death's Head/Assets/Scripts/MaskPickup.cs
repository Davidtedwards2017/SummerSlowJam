using UnityEngine;
using System.Collections;

public class MaskPickup : MonoBehaviour {

    public Transform MaskPrefab;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            //Debug.Log("Player picked up mask " + MaskPrefab.name);
            GiveMask();
        }
    }

    public void GiveMask()
    {
        PlayerController.Instance.AquireMask(MaskPrefab);
        Destroy(gameObject);
    }
}
