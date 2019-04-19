using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hub : MonoBehaviour {

    private Animator anim;
    private AudioSource _audioSource;
    public Slider mainSlider;

    // Use this for initialization
    void Start () {
        _audioSource = GetComponent<AudioSource>();
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        if (MicInput.MicLoudness >= mainSlider.value) {
            anim.SetTrigger("Talk");
            _audioSource.Play();
            new WaitForSeconds(1);
        }
	}
}
