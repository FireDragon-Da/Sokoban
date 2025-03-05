using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int gridWidth = 10;
    public int gridHeight = 5;
    //GameObject player  = GameObject.Find("player");

    private Dictionary<Vector2Int, GameObject> blockPositions = new Dictionary<Vector2Int, GameObject>();

    public bool MoveBlock(GameObject block, Vector2Int direction)
    {
        Vector2Int currentPos = GetBlockPosition(block);
        Vector2Int newPos = currentPos + direction;

        // lemme check the grid's edge
        if (newPos.x < 0 || newPos.x >= gridWidth || newPos.y < 0 || newPos.y >= gridHeight)
        {
            return false;
        }

        // check if occupied
        if (blockPositions.ContainsKey(newPos))
        {
            return false;
        }

        // update
        blockPositions.Remove(currentPos);
        blockPositions[newPos] = block;

        // move the block
        block.transform.position = new Vector3(newPos.x, newPos.y, block.transform.position.z);

        return true;
    }

    public void RegisterBlock(GameObject block, Vector2Int position)
    {
        if (!blockPositions.ContainsKey(position))
        {
            blockPositions[position] = block;
            block.transform.position = new Vector3(position.x, position.y, block.transform.position.z);
        }
    }

    private Vector2Int GetBlockPosition(GameObject block)
    {
        foreach (var entry in blockPositions)
        {
            if (entry.Value == block)
                return entry.Key;
        }
        return Vector2Int.zero;
    }
    public Vector2Int GetPlayerStartPosition()
    {
        GameObject player  = GameObject.Find("player");
        //return new Vector2Int((int)player.transform.position.x+5, (int)player.transform.position.y); // initial state        
        return new Vector2Int(1, 1); // initial state
    }

    public bool IsPositionValid(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth &&
            position.y >= 0 && position.y < gridHeight &&
            !blockPositions.ContainsKey(position); //not overlap
    }

    public Vector3 GridToWorldPosition(Vector2Int gridPos)
    {
        return new Vector3(gridPos.x + 0.5f, gridPos.y + 0.5f, 0); 
    }

}
