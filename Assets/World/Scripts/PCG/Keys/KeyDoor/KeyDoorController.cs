using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class KeyDoorController : MonoBehaviour
{
    private Animator doorAnim;

    [Header("Animation Names")]
    [SerializeField] private string openAnimationName = "DoorOpen";
    [SerializeField] private string closeAnimationName = "DoorClose";

    [Header("Door Locked UI")]
    [SerializeField] private int timeToShowUI = 2;

    [Header("Key Inventory")]
    private bool doorOpen = false;
    [SerializeField] private bool doorUnlocked = false;
    private Color outlineColor;
    private GameObject hintPanel;

    private void Awake()
    {
        doorAnim = gameObject.GetComponent<Animator>();
        // messages.HideText();
    }

    public void PlayAnimation()
    {
        bool animateDoor = false;
        if (doorUnlocked)
        {
            // Show animation
            animateDoor = true;
        }

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
        ShowLockMessage(true);
        yield return new WaitForSeconds(timeToShowUI);
        ShowLockMessage(false);
    }

    private void ShowLockMessage(bool display)
    {
        GameObject hintPanel = GameObject.Find("HintPanel");
        hintPanel.GetComponent<Image>().enabled = display;
        hintPanel.GetComponentInChildren<Text>().enabled = display;
    }

    public void SetUnlocked(bool unlocked) {
        this.doorUnlocked = unlocked;
    }

    public void SetOutlineColor(Color outlineColor){
        this.outlineColor = outlineColor;
    }

    public Color GetOutlineColor(){
        return this.outlineColor;
    }

    public void RemoveOutline(){
        PropOutline outlineScript = GetComponent<PropOutline>();
        Destroy(outlineScript);
    }

    public bool IsUnlocked() {
        return doorUnlocked;
    }
}
