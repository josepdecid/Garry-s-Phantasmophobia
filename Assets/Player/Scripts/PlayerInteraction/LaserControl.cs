using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserControl : MonoBehaviour
{
    [SerializeField] private GameObject laserPrefab = null;
    [SerializeField] private GameObject firePoint = null;

    private LineRenderer line;

    private GameObject spawnedLaser;

    void Start()
    {
        spawnedLaser = Instantiate(laserPrefab, gameObject.transform.position, Quaternion.Euler(0, 180, 0));
        spawnedLaser.transform.parent = gameObject.transform;
        
        spawnedLaser.transform.localScale = new Vector3(1, 1, 1);

        line = spawnedLaser.GetComponentInChildren<LineRenderer>();

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
