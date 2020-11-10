using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomLightings : MonoBehaviour
{
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

        int flashProb = Random.Range(0, 2000);
        if (flashProb < 10) {
            isFlashing = true;
            Debug.Log("Flash");
            StartCoroutine(DoFlash());
        }
    }

    IEnumerator DoFlash() 
    {
        Light light = GetComponent<Light>();
        light.intensity = 3;
        int flashLength = Random.Range(8, 11);
        yield return new WaitForSeconds(flashLength / 10);
        light.intensity = 0;
        isFlashing = false;
    }
}
