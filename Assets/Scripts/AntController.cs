using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : MonoBehaviour {
    public SpriteRenderer antSprite;
    public Sprite antFacingUp;
    public Sprite antFacingDown;
    public Sprite antFacingLeft;
    public Sprite antFacingRight;

    private Grid<TileType> grid;
    private TilemapSpriteUV tilemapVisual;

    private Vector2Int gridPosition;
    private Vector2Int facingDirection = Vector2Int.up;
    private float tileSize = 1f;

    private float moveCooldown = 0.15f;
    private float lastMoveTime = 0f;

    private int energy = 20;
    private bool scanning = false;
    private List<Vector2Int> currentScanArea = new List<Vector2Int>();

    public void Initialize(Grid<TileType> grid, TilemapSpriteUV tilemapVisual, Vector2Int startGridPos, float cellSize) {
        this.grid = grid;
        this.tilemapVisual = tilemapVisual;
        this.gridPosition = startGridPos;
        this.tileSize = cellSize;
        transform.position = GetWorldPosition(gridPosition);
        UpdateSprite();
    }

    void Update() {
        if (scanning) {
            HandleScanInput();
        } else {
            if (Time.time - lastMoveTime >= moveCooldown) {
                HandleMovementInput();
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                EnterScanMode();
            }
        }
    }

    private void HandleMovementInput() {
        Vector2Int moveDir = Vector2Int.zero;

        if (Input.GetKeyDown(KeyCode.W)) moveDir = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.S)) moveDir = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.A)) moveDir = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.D)) moveDir = Vector2Int.right;

        if (moveDir != Vector2Int.zero) {
            facingDirection = moveDir;
            TryMove(moveDir);
            lastMoveTime = Time.time;
            UpdateSprite();
        }
    }

    private void TryMove(Vector2Int direction) {
        Vector2Int newPos = gridPosition + direction;

        if (!grid.IsValidGridPosition(newPos.x, newPos.y)) return;

        TileType targetTile = grid.GetValue(newPos.x, newPos.y);
        if (targetTile == TileType.Rock) return;

        gridPosition = newPos;

        // Handle quicksand fall
        if (targetTile == TileType.Quicksand) {
            Vector2Int below = gridPosition + Vector2Int.down;
            if (grid.IsValidGridPosition(below.x, below.y) &&
                grid.GetValue(below.x, below.y) != TileType.Rock) {
                Vector2Int fallPos = gridPosition + Vector2Int.down * 2;
                if (grid.IsValidGridPosition(fallPos.x, fallPos.y) &&
                    grid.GetValue(fallPos.x, fallPos.y) != TileType.Rock) {
                    gridPosition = fallPos;
                }
            }
        }

        transform.position = GetWorldPosition(gridPosition);
        UseEnergy(1);
    }

    private void EnterScanMode() {
        scanning = true;
        currentScanArea = GetScanArea();
        tilemapVisual.HighlightTiles(currentScanArea);
    }

    private void HandleScanInput() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            scanning = false;
            tilemapVisual.ClearHighlights();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow)) facingDirection = Vector2Int.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) facingDirection = Vector2Int.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) facingDirection = Vector2Int.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) facingDirection = Vector2Int.right;

        if (Input.GetKeyDown(KeyCode.Space)) {
            PerformScan();
            scanning = false;
            tilemapVisual.ClearHighlights();
        } else {
            tilemapVisual.ClearHighlights(); // Clear previous highlight
            currentScanArea = GetScanArea();
            tilemapVisual.HighlightTiles(currentScanArea);
        }

        UpdateSprite();
    }

    private void PerformScan() {
        foreach (Vector2Int pos in currentScanArea) {
            if (!grid.IsValidGridPosition(pos.x, pos.y)) continue;
            
            // Change this line to use the reveal method
            if (grid.GetValue(pos.x, pos.y) == TileType.Quicksand) {
                tilemapVisual.RevealQuicksandTile(pos.x, pos.y);
            }
        }
        UseEnergy(3);
    }
    

    private List<Vector2Int> GetScanArea() {
        List<Vector2Int> area = new List<Vector2Int>();
        Vector2Int forward1 = gridPosition + facingDirection;
        Vector2Int forward2 = forward1 + facingDirection;
        Vector2Int left = new Vector2Int(-facingDirection.y, facingDirection.x);

        area.Add(gridPosition);
        area.Add(forward1);
        area.Add(forward2);
        area.Add(forward2 + left);
        area.Add(forward2 - left);

        return area;
    }

    private void UpdateSprite() {
        if (facingDirection == Vector2Int.up) antSprite.sprite = antFacingUp;
        else if (facingDirection == Vector2Int.down) antSprite.sprite = antFacingDown;
        else if (facingDirection == Vector2Int.left) antSprite.sprite = antFacingLeft;
        else if (facingDirection == Vector2Int.right) antSprite.sprite = antFacingRight;
    }

    private void UseEnergy(int amount) {
        energy -= amount;
        Debug.Log("Energy: " + energy);
        if (energy <= 0) {
            Debug.Log("Game Over: Out of energy");
            // TODO: Trigger game over logic
        }
    }

    private Vector3 GetWorldPosition(Vector2Int pos) {
        return new Vector3(pos.x * tileSize + grid.OriginPosition.x + tileSize / 2,
                           pos.y * tileSize + grid.OriginPosition.y + tileSize / 2,
                           -1f);
    }
}