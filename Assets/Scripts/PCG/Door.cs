using UnityEngine;

public class Door
{
    private Vector2Int innerPos;
    private Vector2Int outerPos;

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
}