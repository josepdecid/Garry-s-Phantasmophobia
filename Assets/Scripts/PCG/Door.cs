using UnityEngine;

public class Door : MonoBehaviour
{
    [SerializeField]
    private Vector2Int innerPos;
    [SerializeField]
    private Vector2Int outerPos;

    //TODO: Add logic for the key and unlocking/locking management

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
}