using UnityEngine;

public class Wall : MonoBehaviour
{
    private void Start()
    {
        // Find the GridManager
        GridManager gridManager = FindFirstObjectByType<GridManager>();
        if (gridManager != null)
        {
            Vector2Int wallPosition = gridManager.WorldToGridPosition(transform.position);

            gridManager.RegisterBlock(gameObject, wallPosition);
        }
    }
}
