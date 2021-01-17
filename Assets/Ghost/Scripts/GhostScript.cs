using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostScript : MonoBehaviour
{
    [SerializeField] private GameObject captureParticles;

    public void Capture()
    {        
        GameObject particles = Instantiate(captureParticles,
                                           gameObject.transform.position + new Vector3(0, 0.5f, 0),
                                           new Quaternion());
        particles.GetComponent<ParticleSystem>().Play();
        GameObject.Find("MainCamera").GetComponent<Sounds>().PlayCapturedSound();

        Destroy(gameObject);
    }
}
