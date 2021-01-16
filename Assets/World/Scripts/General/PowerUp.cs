using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PowerUp : MonoBehaviour
{
    protected Sounds _sounds;

    void Start()
    {
        _sounds = GameObject.Find("MainCamera").GetComponent<Sounds>();
    }

    public abstract void Apply();

    protected abstract void PlaySound();
}
