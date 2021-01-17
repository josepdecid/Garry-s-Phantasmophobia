using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sounds : MonoBehaviour
{   
    [Header("Keys & Doors")]
    [SerializeField] private AudioSource keyPickup;
    [SerializeField] private AudioSource lockedDoor;
    [SerializeField] private AudioSource openDoor;
    [SerializeField] private AudioSource closeDoor;
    [Space]

    [Header("Powerups")]
    [SerializeField] private AudioSource pillPickup;
    [Space]

    [Header("Capture")]
    [SerializeField] private AudioSource startCapture;
    [SerializeField] private AudioSource endCapture;
    [SerializeField] private AudioSource errorCapture;
    [SerializeField] private AudioSource capturedSound;

    public void PlayKeyPickup()
    {
        keyPickup.Play(0);
    }

    public void PlayOpenDoor()
    {
        openDoor.Play(0);
    }

    public void PlayCapturedSound()
    {
        capturedSound.Play(0);
    }

    public void PlayCloseDoor()
    {
        closeDoor.Play(0);
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
