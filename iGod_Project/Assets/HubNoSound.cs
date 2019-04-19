using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HubNoSound : MonoBehaviour {

    private Animator anim;
    public Slider mainSlider;

    // Use this for initialization
    void Start () {
        anim = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        if (MicInput.MicLoudness >= mainSlider.value) {
            anim.SetTrigger("Talk");
            new WaitForSeconds(1);
        }
	}
}
