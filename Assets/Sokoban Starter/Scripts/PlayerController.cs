using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int playerPosition;

    private void Start()
    {
        gridManager = FindFirstObjectByType<GridManager>();
        playerPosition = gridManager.GetPlayerStartPosition();
        UpdatePlayerPosition();
    }

    private void Update()
    {
        Vector2Int moveDirection = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDirection = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) moveDirection = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) moveDirection = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) moveDirection = Vector2Int.right;

        if (moveDirection != Vector2Int.zero)
        {
            Vector2Int newPosition = playerPosition + moveDirection;

            // Boundary detection
            if (!gridManager.IsWithinBounds(newPosition)) return;

            GameObject blockAtTarget = gridManager.GetBlockAtPosition(newPosition);

            if (blockAtTarget != null)
            {
                // Check if block is pushable (Smooth)
                Smooth _smooth = blockAtTarget.GetComponent<Smooth>();
                if (_smooth != null)
                {
                    // Push block
                    if (!_smooth.TryMove(moveDirection)) return;
                }
                else
                {
                    // Check if block is Sticky
                    Sticky _sticky = blockAtTarget.GetComponent<Sticky>();
                    if (_sticky != null)
                    {
                        // Record Sticky's position
                        Vector2Int stickyOldPos = _sticky.GridPosition;

                        // Move Sticky
                        if (!_sticky.TryMove(moveDirection)) return;

                        playerPosition = stickyOldPos;
                        transform.position = gridManager.GridToWorldPosition(playerPosition);

                        // Move adjacent Sticky blocks
                        MoveAdjacentSticky(moveDirection);
                        return; // Complete the move
                    }
                    else
                    {
                        return; // Cannot move if block is not pushable or sticky
                    }
                }
            }

            // Handle Clingy logic
            MoveClingy(moveDirection);

            // Handle Sticky logic
            MoveAdjacentSticky(moveDirection);

            // Move player
            Vector2Int oldPosition = playerPosition;
            playerPosition = newPosition;
            UpdatePlayerPosition();
            transform.position = gridManager.GridToWorldPosition(playerPosition);

            // Notify GridManager about player movement (let Clingy listen)
            gridManager.NotifyBlockMoved(gameObject, oldPosition, playerPosition);
        }
    }

    private void MoveAdjacentSticky(Vector2Int direction)
    {
        Vector2Int[] adjacentPositions =
        {
            playerPosition + Vector2Int.up,
            playerPosition + Vector2Int.down,
            playerPosition + Vector2Int.left,
            playerPosition + Vector2Int.right
        };

        foreach (Vector2Int pos in adjacentPositions)
        {
            GameObject block = gridManager.GetBlockAtPosition(pos);
            if (block != null)
            {
                Sticky sticky = block.GetComponent<Sticky>();
                if (sticky != null && sticky.GridPosition != playerPosition + direction)
                {
                    sticky.TryMove(direction);
                }
            }
        }
    }

    private void MoveClingy(Vector2Int direction)
    {
        Vector2Int pullPosition = playerPosition - direction;
        GameObject block = gridManager.GetBlockAtPosition(pullPosition);

        if (block != null)
        {
            Clingy clingy = block.GetComponent<Clingy>();
            if (clingy != null)
            {
                gridManager.NotifyBlockMoved(gameObject, playerPosition, playerPosition + direction);
            }
        }
    }

    private void UpdatePlayerPosition()
    {
        Vector3 worldPos = gridManager.GridToWorldPosition(playerPosition);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0);
        //print($"Player Position: {playerPosition}"); 
        //print($"Player World Position: {transform.position}"); 
    }
}
