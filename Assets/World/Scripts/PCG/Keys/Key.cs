using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour
{
    protected List<KeyDoorController> doorsLocked;

    public Key() {
        doorsLocked = new List<KeyDoorController>();
    }

    void OnCollisionEnter(Collision collide) {
        Debug.Log(collide.collider.gameObject.name);
        if (collide.collider.gameObject.name == "Capsule") {
            UnlockAllDoors();
            Destroy(gameObject);
        }
    }

    public void AddDoor(KeyDoorController d) {
        doorsLocked.Add(d);
    }

    private void UnlockAllDoors() {
        foreach (KeyDoorController d in this.doorsLocked)
        {
            d.SetUnlocked(true);
        }
    }
}
