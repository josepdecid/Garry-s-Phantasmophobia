using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RandomLightings : MonoBehaviour
{
    public float flashProbPerFrame = 0.005f;
    private int baseRandomRange = 2000;
    public int flashIntensity = 3;
    public float flashLengthMinMilliSecs = 0.8f;
    public float flashLengthMaxMilliSecs = 1.8f;

    private bool isFlashing = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isFlashing) {
            return;
        }

        int flashProb = Random.Range(0, baseRandomRange);
        if (flashProb < flashProbPerFrame * baseRandomRange) {
            isFlashing = true;
            Debug.Log("Flash");
            StartCoroutine(DoFlash());
        }
    }

    IEnumerator DoFlash() 
    {
        // Play sound if not already playing
        AudioSource source = GetComponent<AudioSource>();
        if(!source.isPlaying) {
            source.Play(0);
        }

        // Make flash
        Light light = GetComponent<Light>();
        light.intensity = flashIntensity;
        int flashLength = Random.Range((int) flashLengthMinMilliSecs * 10, (int) flashLengthMaxMilliSecs * 10);
        yield return new WaitForSeconds(flashLength / 10);
        light.intensity = 0;
        isFlashing = false;
    }
}
