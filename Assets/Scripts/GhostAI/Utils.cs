using UnityEngine;

class Utils
{
    public static bool IsTargetVisible(Camera c, GameObject go)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(c);
        Vector3 point = go.transform.position;
        
        foreach (Plane plane in planes)
            if (plane.GetDistanceToPoint(point) < 0)
                return false;
        
        return true;
    }
}