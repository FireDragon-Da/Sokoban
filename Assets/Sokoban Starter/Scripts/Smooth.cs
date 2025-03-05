using UnityEngine;

public class Smooth : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GridManager gridManager = FindObjectOfType<GridManager>();
        blockPosition = gridManager.WorldToGridPosition(transform.position); 
        gridManager.RegisterBlock(gameObject, blockPosition); 
    }

    // Update is called once per frame
    public boolTryToMove (Vector2 direction)
    {
        Verctor2Int newPosition = blockPosition + direction;

        if (gridManager.IsPositionValid(position)){

            
        }
    }
}
