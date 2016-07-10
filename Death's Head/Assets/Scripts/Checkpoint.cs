using UnityEngine;
using System.Collections;

public class Checkpoint : MonoBehaviour {


    public static Checkpoint PrevCheckPoint;

    public void SpwanAtCheckpoint()
    {
        PlayerController.Instance.transform.position = transform.position;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag.Equals("Player"))
        {
            Debug.Log("player crossed check point");
            PrevCheckPoint = this;
        }
    }
}
