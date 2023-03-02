using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneAudioChange : MonoBehaviour
{
    [SerializeField] private AudioSource backgroundAudio;

    [SerializeField] private AudioSource newAudio;
    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        backgroundAudio.clip = newAudio.clip;
        backgroundAudio.Play();
    }
}
