using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateKeyScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.left, 1.0f);
    }
}
