using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Messages : Singleton<Messages>
{ 
    public GameObject hintPanel;

    public void SetHintPanel(GameObject hintPanel)
    {
        this.hintPanel = hintPanel;
    }

    public void ShowText(string message)
    {   
        hintPanel.SetActive(true);
        Debug.Log("Ghost message!");
        Debug.Log(message);
    }

    public void HideText()
    {
        hintPanel.SetActive(false);
    }
}
