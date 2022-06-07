 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds;

    // Start is called before the first frame update
    void Awake()
    {
        foreach (Sound s in sounds)
        {
            s.source= gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;                      
            s.source.reverbZoneMix = s.reverb;
            s.source.playOnAwake = true;
        }
    }

    public void Play(string name)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);
        s.source.Play();
    }

    public float RandomizePitch(float p)
    {
        float wait = 0;
        foreach (var s in sounds)
        {
            s.source.pitch += UnityEngine.Random.Range(-0.01f , 0.01f);
            p = s.source.pitch;
            wait = s.clip.length;
            
        }
        return p;
    }


    private void Update()
    {
        foreach (var s in sounds)
        {
            if (s.isRandomized && (Input.GetKeyDown(KeyCode.F) || Input.GetKeyDown(KeyCode.E)))
            {
                s.source.pitch = RandomizePitch(s.pitch);
                Wait(s.source.clip.length);
                s.source.pitch = 1;
            }
            
            if (!s.isRandomized)
            {
                s.source.pitch = 1;
            }
        }
    }

    private IEnumerable Wait(float time)
    {
        yield return new WaitForSecondsRealtime(time);
    } 
}
