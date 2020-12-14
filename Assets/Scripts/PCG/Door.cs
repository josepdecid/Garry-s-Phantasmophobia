using UnityEngine;
using System.Collections.Generic;

public class Door : MonoBehaviour
{
    [SerializeField]
    protected Vector2Int innerPos;
    [SerializeField]
    protected Vector2Int outerPos;

    public Door(Vector2Int innerPos, Vector2Int outerPos)
    {
        this.innerPos = innerPos;
        this.outerPos = outerPos;
    }

    public Vector2Int GetInnerPos()
    {
        return innerPos;
    }

    public Vector2Int GetOuterPos()
    {
        return outerPos;
    }

    public void SetInnerPos(Vector2Int innerPos)
    {
        this.innerPos = innerPos;
    }

    public void SetOuterPos(Vector2Int outerPos)
    {
        this.outerPos = outerPos;
    }

    public void RemoveWall() {
        foreach(Transform child in this.transform) {
            DestroyImmediate(child.gameObject, true);
        }
    }
}

public class DoorEqualsComparer : IEqualityComparer<Door>
{
    public bool Equals(Door d1, Door d2)
    {
        if (d1 == null && d2 == null) { 
            return true;
        }
        if (d1 == null | d2 == null) { 
            return false;
        }
        if (d1.GetOuterPos() == d2.GetOuterPos() && d1.GetInnerPos() == d2.GetInnerPos()) { 
            return true; 
        }
        return false;
    }
    public int GetHashCode(Door d)
    {
        string code = d.GetOuterPos().ToString()+","+d.GetInnerPos().ToString();
        return code.GetHashCode();
    }
}