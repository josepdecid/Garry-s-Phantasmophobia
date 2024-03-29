using System;
using UnityEngine;
using TMPro;

class Utils
{
    public static bool IsTargetFocused(GameObject playerCamera, string targetId, float distance)
    {
        Vector3 sourcePosition = playerCamera.transform.position;
        Vector3 direction = playerCamera.transform.forward;

        RaycastHit hit;
        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Player"));
        bool isHitting = Physics.Raycast(sourcePosition, direction, out hit, distance, layerMask);

        Debug.DrawRay(sourcePosition, direction * hit.distance, Color.red);     

        if (isHitting && hit.collider.gameObject.GetInstanceID().ToString() == targetId) return true;
        else return false;
    }

    public static bool IsTargetVisible(GameObject player, GameObject target, float fieldOfView, float distance)
    {
        Camera playerCamera = player.GetComponentInChildren<Camera>();
        
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCamera);
        if (GeometryUtility.TestPlanesAABB(planes, target.GetComponent<CapsuleCollider>().bounds))
        {
            Vector3 playerPosition = player.transform.position;
            Vector3 targetPosition = target.transform.position;

            Vector3 dirToTarget = (targetPosition - playerPosition).normalized;

            float dstToTarget = Vector3.Distance(playerPosition, targetPosition);
            
            RaycastHit[] hits = Physics.RaycastAll(playerPosition, dirToTarget, dstToTarget);
            foreach(RaycastHit hit in hits) {
                if (hit.collider.tag == "Wall") {
                    return false;
                }
            }
            return true;
        }

        return false;
    }

    public static bool IsDestinationHidden(GameObject player, Vector3 destination, string targetName, float fieldOfView, float distance)
    {
        Vector3 sourcePosition = player.transform.position;
        Vector3 direction = destination - sourcePosition;
        float angle = Vector3.Angle(direction, player.transform.forward);

        if (Math.Abs(angle) <= fieldOfView)
        {
            RaycastHit hit;
            bool isHitting = Physics.Raycast(sourcePosition, direction, out hit, distance);

            // It collides with an object that is not the NPC
            if (isHitting && hit.collider.gameObject.name != targetName) return true;
        }

        return false;
    }

    public static GameObject GetGhostInFront(GameObject playerCamera, float fieldOfView, float distance)
    {        
        Vector3 sourcePosition = playerCamera.transform.position;
        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");
        LayerMask layerMask = ~(1 << LayerMask.NameToLayer("Player"));

        for (int i = 0; i < ghosts.Length; ++i)
        {
            Vector3 targetPosition = ghosts[i].transform.position;
            Vector3 dirToTarget = (targetPosition - sourcePosition).normalized;
            float dstToTarget = Vector3.Distance(sourcePosition, targetPosition);
            
            float angle = Vector3.Angle(dirToTarget, playerCamera.transform.forward);
            float maxAngle = Mathf.Lerp(fieldOfView / 2f, fieldOfView, Mathf.InverseLerp(0f, dstToTarget, distance));

            if (Math.Abs(angle) <= maxAngle)
            {
                RaycastHit[] hits = Physics.RaycastAll(sourcePosition, dirToTarget, distance, layerMask);

                bool isGhost = false;
                float distToGhost = -1;
                float minDistToWall = Mathf.Infinity;
                foreach(RaycastHit hit in hits) {
                    if (hit.collider.name == ghosts[i].name)
                    {
                        isGhost = true;
                        distToGhost = hit.distance;
                    }
                    else if (hit.collider.tag == "Wall")
                    {
                        if (hit.distance < minDistToWall)
                        {
                            minDistToWall = hit.distance;
                        }
                    }
                }
                
                if (isGhost && distToGhost < minDistToWall) 
                    return ghosts[i];
            }
        }

        return null;
    }
}