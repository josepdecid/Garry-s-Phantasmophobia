using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{   
    [Header("Keys")]
    [SerializeField] private AudioSource keyPickup;
    [SerializeField] private AudioSource lockedDoor;
    [Space]

    [Header("Powerups")]
    [SerializeField] private AudioSource pillPickup;

    [Header("Capture")]
    [SerializeField] private AudioSource startCapture;
    [SerializeField] private AudioSource endCapture;
    [SerializeField] private AudioSource errorCapture;

    public void PlayKeyPickup()
    {
        keyPickup.Play(0);
    }

    public void PlayPillPickup()
    {
        pillPickup.Play(0);
    }

    public void PlayStartCapture()
    {
        startCapture.Play(0);
    }

    public void PlayEndCapture()
    {
        endCapture.Play(0);
    }

    public void PlayErrorCapture(bool overlap)
    {
        if (overlap || !errorCapture.isPlaying)
            errorCapture.Play(0);
    }

    public void PlayLockedDoor()
    {
        lockedDoor.Play(0);
    }
}
