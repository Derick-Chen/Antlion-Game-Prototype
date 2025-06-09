using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : MonoBehaviour {
    private Grid<TileType> grid;
    private float moveCooldown = 0.15f;
    private float lastMoveTime = 0f;

    public Vector2Int currentGridPos;
    public float tileSize = 1f;

    public void Initialize(Grid<TileType> grid, Vector2Int startGridPos, float cellSize) {
        this.grid = grid;
        this.currentGridPos = startGridPos;
        this.tileSize = cellSize;

        UpdateWorldPosition();
    }

    void Update() {
        if (Time.time - lastMoveTime < moveCooldown) return;

        Vector2Int input = Vector2Int.zero;
        if (Input.GetKeyDown(KeyCode.W)) input = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) input = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) input = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) input = Vector2Int.right;

        if (input != Vector2Int.zero) {
            TryMove(input);
            lastMoveTime = Time.time;
        }
    }

    void TryMove(Vector2Int direction) {
        Vector2Int newPos = currentGridPos + direction;

        if (!grid.IsValidGridPosition(newPos.x, newPos.y)) return;

        TileType targetTile = grid.GetValue(newPos.x, newPos.y);

        if (targetTile == TileType.Rock) return;

        currentGridPos = newPos;
        UpdateWorldPosition();

        // Check for quicksand fall
        if (targetTile == TileType.Quicksand) {
            Vector2Int fallPos = currentGridPos + Vector2Int.down * 2;

            // Prevent falling if rock is directly below
            Vector2Int belowTile = currentGridPos + Vector2Int.down;
            if (grid.IsValidGridPosition(belowTile.x, belowTile.y)) {
                if (grid.GetValue(belowTile.x, belowTile.y) == TileType.Rock) return;
            }

            if (grid.IsValidGridPosition(fallPos.x, fallPos.y) &&
                grid.GetValue(fallPos.x, fallPos.y) != TileType.Rock) {
                currentGridPos = fallPos;
                UpdateWorldPosition();
            }
        }
    }

    void UpdateWorldPosition() {
        transform.position = new Vector3(
            currentGridPos.x * tileSize + grid.OriginPosition.x + tileSize / 2,
            currentGridPos.y * tileSize + grid.OriginPosition.y + tileSize / 2,
            -1f // Put above tile layer
        );
    }
}
