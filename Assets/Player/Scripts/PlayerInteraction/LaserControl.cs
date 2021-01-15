using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserControl : MonoBehaviour
{
    [SerializeField] private GameObject cameraComponent = null;
    [SerializeField] private GameObject laserPrefab = null;
    [SerializeField] private GameObject firePoint = null;

    private LineRenderer line;
    private ParticleSystem particles;

    private GameObject spawnedLaser;

    void Start()
    {
        spawnedLaser = Instantiate(laserPrefab, firePoint.transform.position, Quaternion.Euler(0, 180, 0));
        spawnedLaser.transform.parent = cameraComponent.transform;
        
        spawnedLaser.transform.localScale = new Vector3(1, 1, 1);

        line = spawnedLaser.GetComponentInChildren<LineRenderer>();
        particles = spawnedLaser.GetComponentInChildren<ParticleSystem>();

        // Set spawn particles with manual offset to be in front of the gun
        particles.transform.position = firePoint.transform.position;
        particles.transform.position += new Vector3(0.4f, 0.4f, 1.2f);

        DisableLaser();
    }

    public void EnableLaser()
    {
        spawnedLaser.SetActive(true);
    }

    public void UpdateLaser(Vector3 targetPoint)
    {
        line.SetPosition(0, firePoint.transform.position);
        line.SetPosition(1, targetPoint);
    }
    
    public void DisableLaser()
    {
        spawnedLaser.SetActive(false);
    }
}
