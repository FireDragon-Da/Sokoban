using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private GridManager gridManager;
    private Vector2Int playerPosition;

    private void Start()
    {
        gridManager = FindObjectOfType<GridManager>();
        playerPosition = gridManager.GetPlayerStartPosition();
        UpdatePlayerPosition(); 
    }

    private void Update()
    {
        Vector2Int moveDirection = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W))
            moveDirection = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S))
            moveDirection = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A))
            moveDirection = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D))
            moveDirection = Vector2Int.right;

        if (moveDirection != Vector2Int.zero)
        {
            Vector2Int newPosition = playerPosition + moveDirection;
            if (gridManager.IsPositionValid(newPosition))
            {
                playerPosition = newPosition;
                UpdatePlayerPosition();
            }
        }
        print($"Player Position: {playerPosition}"); 
        print($"Player World Position: {transform.position}"); 
    }

    private void UpdatePlayerPosition()
    {
        Vector3 worldPos = gridManager.GridToWorldPosition(playerPosition);
        transform.position = new Vector3(worldPos.x, worldPos.y, 0); 
    }
}
