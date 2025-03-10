using UnityEngine;

public class Smooth : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int blockPosition;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        blockPosition = gridManager.WorldToGridPosition(transform.position);
        gridManager.RegisterBlock(gameObject, blockPosition);
    }

    public bool TryMove(Vector2Int direction)
    {
        Vector2Int newPosition = blockPosition + direction;

        // Check if new position is valid
        if (!gridManager.IsPositionValid(newPosition))
        {
            return false;
        }

        // Attempt to move Sticky
        bool stickyMoved = MoveStickyBlocks(direction);

        // Move Smooth if Sticky moved
        if (stickyMoved)
        {
            MoveTo(newPosition);
            return true;
        }

        // Move Smooth if front is empty
        if (gridManager.GetBlockAtPosition(newPosition) == null)
        {
            MoveTo(newPosition);
            return true;
        }

        return false;
    }

    // Handle movement logic
    private void MoveTo(Vector2Int newPosition)
    {
        gridManager.UpdateBlockPosition(gameObject, blockPosition, newPosition);
        blockPosition = newPosition;
        transform.position = gridManager.GridToWorldPosition(blockPosition);
    }

    private bool MoveStickyBlocks(Vector2Int direction)
    {
        bool anyStickyMoved = false;

        Vector2Int[] adjacentPositions =
        {
            blockPosition + Vector2Int.up,
            blockPosition + Vector2Int.down,
            blockPosition + Vector2Int.left,
            blockPosition + Vector2Int.right
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            GameObject adjacentBlock = gridManager.GetBlockAtPosition(pos);
            if (adjacentBlock != null)
            {
                Sticky stickyBlock = adjacentBlock.GetComponent<Sticky>();
                if (stickyBlock != null && stickyBlock.TryMove(direction))
                {
                    anyStickyMoved = true;
                }
            }
        }

        return anyStickyMoved;
    }

    private bool HasAdjacentSticky()
    {
        Vector2Int[] adjacentPositions =
        {
            blockPosition + Vector2Int.up,
            blockPosition + Vector2Int.down,
            blockPosition + Vector2Int.left,
            blockPosition + Vector2Int.right
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            GameObject adjacentBlock = gridManager.GetBlockAtPosition(pos);
            if (adjacentBlock != null && adjacentBlock.GetComponent<Sticky>() != null)
            {
                return true;
            }
        }

        return false;
    }

    private void MoveClingyBlocks(Vector2Int direction)
    {
        Vector2Int pullPosition = blockPosition - direction;
        GameObject adjacentBlock = gridManager.GetBlockAtPosition(pullPosition);

        if (adjacentBlock != null)
        {
            Clingy clingyBlock = adjacentBlock.GetComponent<Clingy>();
            if (clingyBlock != null)
            {
                clingyBlock.TryMove(direction);
            }
        }
    }
}
