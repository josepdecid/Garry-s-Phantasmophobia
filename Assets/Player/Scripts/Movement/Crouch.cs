using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crouch : MonoBehaviour
{
    public KeyCode crouchKey = KeyCode.C;
    private CharacterController characterController;
    private bool isCrouched = false;
    private float originalHeight;
    private float crouchHeight = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
        originalHeight = characterController.height;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(crouchKey))
        {
            isCrouched = !isCrouched;
            UpdateHeight();
        }
    }

    void UpdateHeight()
    {
        if (isCrouched)
        {
            characterController.height = crouchHeight;
        }
        else
        {
            characterController.height = originalHeight;
        }
    }
}
