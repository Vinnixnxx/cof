using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public string name;
    [Range(0f,1f)]
    public float volume;
    [Range(0f,3f)]
    public float pitch = 1;
    [Range(0f, 1f)]
    public float reverb;
    public bool playOnAwake;
    public bool isRandomized;
    [HideInInspector]
    public AudioSource source;


    

}
