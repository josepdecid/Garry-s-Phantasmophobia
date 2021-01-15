using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class IncreasePowerUp : PowerUp
{    
    public override void Apply() {
        FirstPersonController playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<FirstPersonController>();
        playerController.m_WalkSpeed *= 1.25f;
        playerController.m_RunSpeed *= 1.25f;
    }
}
