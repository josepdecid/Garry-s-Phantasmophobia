using UnityEngine;

public class BasicDoorController : MonoBehaviour
{
    private Animator doorAnim;

    private bool doorOpen = false;

    [SerializeField] private string openAnimationName = "DoorOpen";
    [SerializeField] private string closeAnimationName = "DoorClose";

    private void Awake()
    {
        doorAnim = gameObject.GetComponent<Animator>();
    }

    public void PlayAnimation()
    {
        if (!doorOpen)
        {
            doorAnim.Play(openAnimationName, 0, 0.0f);
            doorOpen = true;
        }

        else
        {
            doorAnim.Play(closeAnimationName, 0, 0.0f);
            doorOpen = false;
        }
    }
}
