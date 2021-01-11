using System.Collections;
using UnityEngine;

public class KeyDoorController : MonoBehaviour
{
    private Animator doorAnim;

    [Header("Animation Names")]
    [SerializeField] private string openAnimationName = "DoorOpen";
    [SerializeField] private string closeAnimationName = "DoorClose";

    [Header("Door Locked UI")]
    [SerializeField] private int timeToShowUI = 1;
    [SerializeField] private GameObject showDoorLockedUI = null;

    [Header("Key Inventory")]
    private bool doorOpen = false;
    [SerializeField] private bool doorUnlocked = false;
    private KeyDoorInventory keyInventory = null;

    private void Awake()
    {
        doorAnim = gameObject.GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        // // Get key inventory from FPS controller
        // keyInventory = (KeyDoorInventory) GameObject.Find("FPSController(Clone)/KeyInventory").GetComponent<KeyDoorInventory>();

        bool animateDoor = false;

        // // If door still locked, but player has a key
        // if (keyInventory.hasSkeletonKey && !doorUnlocked)
        // {
        //     // Unlock the door
        //     doorUnlocked = true;
        //     // Use the key of the user
        //     keyInventory.hasSkeletonKey = false;
        //     // Show animation
        //     animateDoor = true;
        // }

        // ...door already unlocked
        if (doorUnlocked)
        {
            // Show animation
            animateDoor = true;
        }

        // .. door locked and no key
        else
        {
            StartCoroutine(ShowDoorLocked());
        }

        // Play animation for opening / closing an unlocked door
        if (animateDoor) 
        {
            if (!doorOpen) {
                doorAnim.Play(openAnimationName, 0, 0.0f);
                doorOpen = true;
            }
            else {
                doorAnim.Play(closeAnimationName, 0, 0.0f);
                doorOpen = false;
            }
        }
    }

    IEnumerator ShowDoorLocked()
    {
        showDoorLockedUI.SetActive(true);
        yield return new WaitForSeconds(timeToShowUI);
        showDoorLockedUI.SetActive(false);
    }

    public void SetUnlocked(bool unlocked) {
        this.doorUnlocked = unlocked;
    }
}
