using UnityEngine;
using UnityEngine.UI;

public class KeyDoorRaycast : MonoBehaviour
{
    [Header("Raycast Parameters")]
    [SerializeField] private int rayLength = 5;
    [SerializeField] private LayerMask layerMaskInteract;
    [SerializeField] private string exludeLayerName = null;

    private KeyDoorController raycasted_door;
    private GameObject raycastedKey;
    // [SerializeField] private KeyDoorInventory keyInventory = null;

    [Header("Key Codes")]
    [SerializeField] private KeyCode openDoorKey = KeyCode.Mouse0;

    [Header("UI Parameters")]
    [SerializeField] private Image crosshair = null;
    private bool isCrosshairActive;
    private bool doOnce;

    private const string interactableTagKey = "Key";
    private const string interactableTagDoor = "InteractiveObject";

    private void Update()
    {
        RaycastHit hit;
        Vector3 fwd = transform.TransformDirection(Vector3.forward);

        int mask = 1 << LayerMask.NameToLayer(exludeLayerName) | layerMaskInteract.value;


        if (Physics.Raycast(transform.position, fwd, out hit, rayLength, mask))
        {
             if (hit.collider.CompareTag(interactableTagDoor))
             {
                if (!doOnce)
                {
                    raycasted_door = hit.collider.gameObject.GetComponent<KeyDoorController>();
                    CrosshairChange(true);
                }

                isCrosshairActive = true;
                doOnce = true;

                if (Input.GetKeyDown(openDoorKey))
                {
                    raycasted_door.PlayAnimation();
                }
             }

            else if (hit.collider.CompareTag(interactableTagKey))
            {
                if (!doOnce)
                {
                    CrosshairChange(true);
                    raycastedKey = hit.collider.gameObject;
                }

                isCrosshairActive = true;
                doOnce = true;

                if (Input.GetKeyDown(openDoorKey))
                {
                    // Get key inventory from FPS controller
                    KeyDoorInventory keyInventory = (KeyDoorInventory) GameObject.Find("FPSController/KeyInventory").GetComponent<KeyDoorInventory>();

                    // Collect a key if there is no key in inventory
                    if (!keyInventory.hasSkeletonKey) {
                        keyInventory.hasSkeletonKey = true;
                        raycastedKey.SetActive(false);
                    }
                }
            }
        }

        else
        {
            if (isCrosshairActive)
            {
                CrosshairChange(false);
                doOnce = false;
            }
        }
    }

    void CrosshairChange(bool on)
    {
        if (on && !doOnce)
        {
            crosshair.color = Color.red;
        }
        else
        {
            crosshair.color = Color.white;
            isCrosshairActive = false;
        }
    }
}