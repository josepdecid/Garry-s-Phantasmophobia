using UnityEngine;

public class ButtonDoorController : MonoBehaviour
{
    [Header("Door Object")]
    [SerializeField] private Animator doorAnim = null;

    private bool doorOpen = false;

    [Header("Door Animation Names")]
    [SerializeField] private string openAnimationName = "DoorOpen";
    [SerializeField] private string closeAnimationName = "DoorClose";

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
