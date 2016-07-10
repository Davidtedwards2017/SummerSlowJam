using UnityEngine;

public class CameraFade : MonoBehaviour {

    private Animator m_Animator;
    public static CameraFade Instance;

    void Awake()
    {
        Instance = this;
    }

	void Start () {
        m_Animator = GetComponent<Animator>();
	}

    public void SetFade(bool value)
    {
        m_Animator.SetBool("FadeOut", value);
    }

}
