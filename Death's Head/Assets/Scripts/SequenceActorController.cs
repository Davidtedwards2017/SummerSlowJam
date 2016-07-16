using UnityEngine;
using System.Collections;
using Spine.Unity;

public class SequenceActorController : MonoBehaviour {

    private SkeletonAnimation m_Animator;
    private Renderer m_Renderer;
        // Use this for initialization
	void Awake () {
        m_Animator = GetComponentInChildren<SkeletonAnimation>();
        m_Renderer = GetComponentInChildren<Renderer>();
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    public void SetVisable(bool visable)
    {
        m_Renderer.enabled = visable;
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
