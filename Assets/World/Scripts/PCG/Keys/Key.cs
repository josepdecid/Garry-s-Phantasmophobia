using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    protected List<KeyDoorController> doorsLocked;
    private Color outlineColor;

    public Key() {
        doorsLocked = new List<KeyDoorController>();
    }

    public void AddDoor(KeyDoorController d) {
        doorsLocked.Add(d);
        d.SetOutlineColor(outlineColor);
    }

    public Color GetOutlineColor(){
        return outlineColor;
    }

    public void SetOutlineColor(Color outlineColor){
        this.outlineColor = outlineColor;
    }

    public void UnlockAllDoors() {
        foreach (KeyDoorController d in this.doorsLocked)
        {
            d.SetUnlocked(true);
            d.RemoveOutline();
        }
    }
}
