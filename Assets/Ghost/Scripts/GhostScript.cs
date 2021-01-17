using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostScript : MonoBehaviour
{
    [SerializeField] private GameObject captureParticles = null;
    [SerializeField] private AudioClip[] ghostSounds = null;
    [SerializeField] private float timeBetweenSounds = 10;
    [SerializeField] private float timeRandomSounds = 30;

    private AudioSource audioSource;
    private float timeSinceLastSound = 0f;

    void Start()
    {
        timeSinceLastSound = timeBetweenSounds + 1;
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    void Update()
    {
        timeSinceLastSound += Time.deltaTime;
        if (timeSinceLastSound > timeRandomSounds)
            PlayRandomSound();
    }

    public void Capture()
    {        
        GameObject particles = Instantiate(captureParticles,
                                           gameObject.transform.position + new Vector3(0, 0.5f, 0),
                                           new Quaternion());
        particles.GetComponent<ParticleSystem>().Play();
        GameObject.Find("MainCamera").GetComponent<Sounds>().PlayCapturedSound();

        Destroy(gameObject);
    }

    public void PlayRandomSound()
    {
        if (timeSinceLastSound > timeBetweenSounds)
        {
            int idx = Random.Range(0, ghostSounds.Length);
            audioSource.PlayOneShot(ghostSounds[idx]);

            timeSinceLastSound = 0;
        }
    }
}
