using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class SoundController : MonoBehaviour {

    public static SoundController Instance;

    public SoundInfo[] Sounds;

    [System.Serializable]
    public class SoundInfo
    {
        public string key;
        public AudioClip Clip;
        public float Vol;
    }
    
    void Awake()
    {
        Instance = this;
    }

    public AudioSource PlaySound(string key)
    {
        SoundInfo soundInfo = GetSoundInfo(key);

        if (soundInfo != null)
        {
            return SpawnAudioSource(soundInfo);
        }

        return null;
    }

    private SoundInfo GetSoundInfo(string key)
    {
        var soundInfo = Sounds.FirstOrDefault(s => s.key.Equals(key, System.StringComparison.CurrentCultureIgnoreCase));
        if(soundInfo != null)
        { 
            return soundInfo;
        }

        return null;
    }

    private AudioSource SpawnAudioSource(SoundInfo soundInfo)
    {
        GameObject go = new GameObject(soundInfo.key + " audio source", new[] { typeof(AudioSource) });
        AudioSource audioSource = go.GetComponent<AudioSource>();
        audioSource.clip = soundInfo.Clip;
        audioSource.volume = soundInfo.Vol;
        audioSource.spatialBlend = 0;
        audioSource.Play();

        Destroy(go, soundInfo.Clip.length);
        return audioSource;
    }
}
