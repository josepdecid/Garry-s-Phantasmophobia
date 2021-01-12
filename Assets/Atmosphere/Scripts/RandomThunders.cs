using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomThunders : MonoBehaviour
{
    public float flashProbPerFrame = 0.005f;
    private int baseRandomRange = 2000;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        AudioSource source = GetComponent<AudioSource>();
        if(source.isPlaying) {
            return;
        }

        int flashProb = Random.Range(0, baseRandomRange);
        if (flashProb < flashProbPerFrame * baseRandomRange) {
            // Play sound if not already playing
            source.Play(0);
        }
    }
}
