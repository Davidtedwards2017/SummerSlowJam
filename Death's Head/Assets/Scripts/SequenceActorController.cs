using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SequenceActorController : MonoBehaviour {

    private SkeletonAnimation m_Animator;
        // Use this for initialization
	void Start () {
        m_Animator = GetComponentInChildren<SkeletonAnimation>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void MoveTo(Transform anchor, float duration)
    {
        StartCoroutine(MoveToAsync(anchor, duration));
    }

    public IEnumerator MoveToAsync(Transform anchor, float duration)
    {
        Vector3 startPos = transform.position;
        Vector3 endPos = anchor.transform.position;

        float elapsedTime = 0;
        float ratio = elapsedTime / duration;
        while (ratio < 1f)
        {
            elapsedTime += Time.deltaTime;
            ratio = elapsedTime / duration;
            transform.position = Vector3.Lerp(startPos, endPos, ratio);
            yield return null;
        }

    }

    public void StartAnimation(string name, float timeScale, bool looping)
    {
        m_Animator.timeScale = timeScale;
        m_Animator.loop = looping;
        m_Animator.AnimationName = name;
        
    }

}
