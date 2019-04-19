using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundControl : MonoBehaviour
{

    private AudioSource _audioSource;

    void Start()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (MicInput.MicLoudness >= 0.01) {
            _audioSource.Play();
            new WaitForSeconds(2);
        }

    }
}


