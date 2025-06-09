using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapSpriteUV : MonoBehaviour {
    private TileSpriteData[] tileSprites;
    private Grid<TileType> grid;
    private SpriteRenderer[,] tileRenderers;

    public void Initialize(Grid<TileType> grid, TileSpriteData[] tileSprites) {
        this.grid = grid;
        this.tileSprites = tileSprites;
        CreateTilemap();
        grid.OnValueChanged += UpdateTileVisual;
        
        // Initialize all tiles AFTER setting up visuals
        InitializeAllTiles();
    }

    private void CreateTilemap() {
        int width = grid.Width;
        int height = grid.Height;
        tileRenderers = new SpriteRenderer[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                CreateTile(x, y);
            }
        }
    }

    private void CreateTile(int x, int y) {
        GameObject tileGO = new GameObject($"Tile_{x}_{y}", typeof(SpriteRenderer));
        tileGO.transform.SetParent(transform);
        
        // Position with z=0 for visibility
        tileGO.transform.position = new Vector3(
            x * grid.CellSize + grid.OriginPosition.x + grid.CellSize/2,
            y * grid.CellSize + grid.OriginPosition.y + grid.CellSize/2,
            0
        );

        SpriteRenderer renderer = tileGO.GetComponent<SpriteRenderer>();
        tileRenderers[x, y] = renderer;
        
        // REMOVED grid.SetValue call from here
    }

    private void InitializeAllTiles() {
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                // Set to Sand and trigger visual update
                grid.SetValue(x, y, TileType.Sand);
            }
        }
    }

    private void UpdateTileVisual(int x, int y, TileType tileType) {
        if (tileRenderers == null || tileSprites == null) return;
        
        foreach (TileSpriteData data in tileSprites) {
            if (data.tileType == tileType) {
                tileRenderers[x, y].sprite = data.sprite;
                tileRenderers[x, y].color = data.color;
                return;
            }
        }
    }

    private void OnDestroy() {
        if (grid != null) {
            grid.OnValueChanged -= UpdateTileVisual;
        }
    }
}