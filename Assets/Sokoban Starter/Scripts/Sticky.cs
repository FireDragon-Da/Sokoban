using UnityEngine;

public class Sticky : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int stickyPosition;
    private bool isMoving = false; // Prevent duplicate movement calls

    public Vector2Int GridPosition => stickyPosition;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        stickyPosition = gridManager.WorldToGridPosition(transform.position);
        gridManager.RegisterBlock(gameObject, stickyPosition);
    }

    public bool TryMove(Vector2Int direction)
    {
        if (isMoving) return false; // Prevent duplicate calls
        isMoving = true;

        Vector2Int newPosition = stickyPosition + direction;

        // Check boundaries
        if (!gridManager.IsWithinBounds(newPosition))
        {
            isMoving = false;
            return false;
        }

        // Check if the target position is blocked
        GameObject targetBlock = gridManager.GetBlockAtPosition(newPosition);
        if (targetBlock != null)
        {
            // Cannot move if it is a wall
            if (targetBlock.GetComponent<Wall>() != null)
            {
                isMoving = false;
                return false;
            }

            // Sticky follows naturally
            if (targetBlock.GetComponent<Sticky>() != null)
            {
                bool result = targetBlock.GetComponent<Sticky>().TryMove(direction);
                isMoving = false;
                return result;
            }

            // Sticky cannot push Clingy but can pull it
            if (targetBlock.GetComponent<Clingy>() != null)
            {
                isMoving = false;
                return false;
            }

            // Smooth can be pushed
            if (targetBlock.GetComponent<Smooth>() != null)
            {
                if (!TryPush(targetBlock, direction))
                {
                    isMoving = false;
                    return false;
                }
            }
        }

        // Attempt to pull Clingy
        PullClingy(direction);

        // Update Sticky position
        gridManager.UpdateBlockPosition(gameObject, stickyPosition, newPosition);
        stickyPosition = newPosition;
        transform.position = gridManager.GridToWorldPosition(stickyPosition);

        // Notify Clingy about movement
        NotifyAdjacentBlocksMoved(stickyPosition - direction, stickyPosition);

        isMoving = false;
        return true;
    }

    private bool TryPush(GameObject block, Vector2Int direction)
    {
        Vector2Int pushPosition = stickyPosition + direction * 2;

        // Check if push position is within bounds
        if (!gridManager.IsWithinBounds(pushPosition))
            return false;

        // Check if push position is blocked
        GameObject pushTarget = gridManager.GetBlockAtPosition(pushPosition);
        if (pushTarget != null)
        {
            // Cannot push if it is a wall
            if (pushTarget.GetComponent<Wall>() != null)
                return false;

            // Sticky cannot push Clingy
            if (pushTarget.GetComponent<Clingy>() != null)
                return false;

            // Recursively attempt to push the front block
            if (!TryPush(pushTarget, direction))
                return false;
        }

        // Execute push
        gridManager.UpdateBlockPosition(block, stickyPosition + direction, pushPosition);
        block.transform.position = gridManager.GridToWorldPosition(pushPosition);

        return true;
    }

    private void NotifyAdjacentBlocksMoved(Vector2Int oldPosition, Vector2Int newPosition)
    {
        foreach (Clingy clingy in FindObjectsOfType<Clingy>())
        {
            clingy.OnAdjacentBlockMoved(gameObject, oldPosition, newPosition);
        }
    }

    private void PullClingy(Vector2Int direction)
    {
        Vector2Int pullPosition = stickyPosition - direction; // Clingy may be behind Sticky
        GameObject blockBehind = gridManager.GetBlockAtPosition(pullPosition);

        if (blockBehind != null && blockBehind.GetComponent<Clingy>() != null)
        {
            blockBehind.GetComponent<Clingy>().TryMove(direction);
        }
    }
}
