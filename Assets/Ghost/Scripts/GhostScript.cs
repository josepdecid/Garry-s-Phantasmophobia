using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GhostScript : MonoBehaviour
{
    CaptureScript capture;
    Vector3 playerPos;

    private float energy;

    private void Start()
    {
        capture = FindObjectOfType<CaptureScript>();
    }

    public void Capture()
    {
        Destroy(gameObject);
    }            

    public void Damage(float angle, Vector3 axis)
    {
        float damage = Remap(angle, 130, 180, .5f, .1f);
        energy = Mathf.Max(0,energy - (.6f - damage));
    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void Update()
    {
        // if (escaping) CanvasManager.instance.UpdateText(head.position, ((int)energy).ToString());
    }
}
