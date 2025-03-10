using UnityEngine;

public class Clingy : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int clingyPosition;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        clingyPosition = gridManager.WorldToGridPosition(transform.position);
        gridManager.RegisterBlock(gameObject, clingyPosition);
    }

    public bool TryMove(Vector2Int direction)
    {
        Vector2Int targetPosition = clingyPosition + direction;

        // Check if the target position is valid and empty
        if (gridManager.IsWithinBounds(targetPosition) && gridManager.GetBlockAtPosition(targetPosition) == null)
        {
            Vector2Int oldPosition = clingyPosition;

            // Update position
            gridManager.UpdateBlockPosition(gameObject, clingyPosition, targetPosition);
            clingyPosition = targetPosition;
            transform.position = gridManager.GridToWorldPosition(clingyPosition);

            Debug.Log($"[Clingy] Moved from {oldPosition} to {clingyPosition}");

            // Try to pull Sticky blocks
            PullSticky(direction);

            return true;
        }

        return false;
    }

    public void OnAdjacentBlockMoved(GameObject mover, Vector2Int oldPosition, Vector2Int newPosition)
    {
        Vector2Int moveDirection = newPosition - oldPosition;

        Debug.Log($"[Clingy] {mover.name} moved from {oldPosition} to {newPosition}, Clingy at {clingyPosition}");

        // If the mover moves away from Clingy, Clingy is pulled
        if (oldPosition == clingyPosition + moveDirection)
        {
            if (TryMove(moveDirection))
            {
                // If Clingy was pulled, check if it affects Sticky blocks
                PullSticky(moveDirection);
            }
        }
    }

    private void PullSticky(Vector2Int direction)
    {
        Vector2Int stickyPosition = clingyPosition + direction;
        GameObject stickyObject = gridManager.GetBlockAtPosition(stickyPosition);

        if (stickyObject != null)
        {
            Sticky sticky = stickyObject.GetComponent<Sticky>();
            if (sticky != null)
            {
                Debug.Log($"[Clingy] Found Sticky at {stickyPosition}, attempting to move...");

                // Ensure Sticky moves successfully
                if (sticky.TryMove(direction))
                {
                    Debug.Log($"[Clingy] Successfully pulled Sticky at {stickyPosition}.");
                }
                else
                {
                    Debug.Log($"[Clingy] Sticky at {stickyPosition} could not move!");
                }
            }
        }
        else
        {
            Debug.Log($"[Clingy] No Sticky found at {stickyPosition}");
        }
    }
}
