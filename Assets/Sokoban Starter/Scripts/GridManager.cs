using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    private GridMaker gridMaker;
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
        UpdateBlockPosition(block, currentPos, newPos);
        blockPositions.Remove(currentPos);
        blockPositions[newPos] = block;

        // move the block
        block.transform.position = new Vector3(newPos.x, newPos.y, block.transform.position.z);
        NotifyBlockMoved(block, currentPos, newPos);

        return true;
    }

    public void NotifyBlockMoved(GameObject movingObject, Vector2Int currentPos, Vector2Int newPos)
    {
        foreach (Clingy clingy in FindObjectsOfType<Clingy>())
        {
            clingy.OnAdjacentBlockMoved(movingObject, currentPos, newPos);
        }
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
        //GameObject player  = GameObject.Find("player");
        //return new Vector2Int((int)player.transform.position.x+5, (int)player.transform.position.y); // initial state        
        return new Vector2Int(0, 5); // initial state
    }

    // Get the block at grid position
    public GameObject GetBlockAtPosition(Vector2Int position)
    {
        return blockPositions.ContainsKey(position) ? blockPositions[position] : null;
    }

    // Update position 
    public void UpdateBlockPosition(GameObject block, Vector2Int oldPos, Vector2Int newPos)
    {
        if (blockPositions.ContainsKey(oldPos))
        {
            //Debug.Log($"[GridManager] Moving {block.name} from {oldPos} to {newPos}");
            blockPositions.Remove(oldPos);
        }

        blockPositions[newPos] = block;
    }



    public bool IsPositionValid(Vector2Int position)
{
    return IsWithinBounds(position) && GetBlockAtPosition(position) == null;
}


    public Vector3 GridToWorldPosition(Vector2Int gridPos)
    {

        return new Vector3((gridPos.x - gridWidth/2) + 0.5f, (gridPos.y - gridHeight/2), 0); 
    }

    public bool IsWithinBounds(Vector2Int position)
    {
        return position.x >= 0 && position.x < gridWidth &&
            position.y >= 0 && position.y < gridHeight;
    }

    public Vector2Int WorldToGridPosition(Vector3 worldPos)
    {
        int x = Mathf.FloorToInt(worldPos.x + (gridWidth / 2));
        int y = Mathf.FloorToInt(worldPos.y + (gridHeight / 2));
        return new Vector2Int(x, y);
    }

}
