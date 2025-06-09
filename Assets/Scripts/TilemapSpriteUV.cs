using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TilemapSpriteUV : MonoBehaviour {
    private TileSpriteData[] tileSprites;
    private Grid<TileType> grid;
    private SpriteRenderer[,] tileRenderers;
    private Color[,] originalColors;

    // Track revealed quicksand tiles
    private HashSet<Vector2Int> revealedQuicksandTiles = new HashSet<Vector2Int>();

    [Header("Highlight Color")]
    public Color highlightColor = new Color(1f, 1f, 0f, 0.5f); // Yellowish with transparency

    public void Initialize(Grid<TileType> grid, TileSpriteData[] tileSprites) {
        this.grid = grid;
        this.tileSprites = tileSprites;
        CreateTilemap();
        grid.OnValueChanged += UpdateTileVisual;
    }

    private void CreateTilemap() {
        int width = grid.Width;
        int height = grid.Height;
        tileRenderers = new SpriteRenderer[width, height];
        originalColors = new Color[width, height];

        for (int x = 0; x < width; x++) {
            for (int y = 0; y < height; y++) {
                CreateTile(x, y);
            }
        }
    }

    private void CreateTile(int x, int y) {
        GameObject tileGO = new GameObject($"Tile_{x}_{y}", typeof(SpriteRenderer));
        tileGO.transform.SetParent(transform);

        tileGO.transform.position = new Vector3(
            x * grid.CellSize + grid.OriginPosition.x + grid.CellSize / 2,
            y * grid.CellSize + grid.OriginPosition.y + grid.CellSize / 2,
            0
        );

        SpriteRenderer renderer = tileGO.GetComponent<SpriteRenderer>();
        tileRenderers[x, y] = renderer;
    }

    private void UpdateTileVisual(int x, int y, TileType tileType) {
        if (tileRenderers == null || tileSprites == null) return;

        // Determine visual type - hide unrevealed quicksand
        TileType visualType = tileType;
        Vector2Int pos = new Vector2Int(x, y);
        
        if (tileType == TileType.Quicksand && !revealedQuicksandTiles.Contains(pos)) {
            visualType = TileType.Sand;
        }

        // Apply appropriate sprite and color
        foreach (TileSpriteData data in tileSprites) {
            if (data.tileType == visualType) {
                tileRenderers[x, y].sprite = data.sprite;
                tileRenderers[x, y].color = data.color;
                originalColors[x, y] = data.color;
                return;
            }
        }
    }

    // Highlight tiles for scan preview
    public void HighlightTiles(List<Vector2Int> tiles) {
        foreach (Vector2Int pos in tiles) {
            if (!grid.IsValidGridPosition(pos.x, pos.y)) continue;

            SpriteRenderer renderer = tileRenderers[pos.x, pos.y];
            renderer.color = highlightColor;
        }
    }

    // Revert all tiles back to their original color
    public void ClearHighlights() {
        for (int x = 0; x < grid.Width; x++) {
            for (int y = 0; y < grid.Height; y++) {
                tileRenderers[x, y].color = originalColors[x, y];
            }
        }
    }

    // Reveal a quicksand tile (change from sand to quicksand texture)
    public void RevealQuicksandTile(int x, int y) {
        Vector2Int pos = new Vector2Int(x, y);
        if (grid.IsValidGridPosition(x, y) && grid.GetValue(x, y) == TileType.Quicksand) {
            revealedQuicksandTiles.Add(pos);
            UpdateTileVisual(x, y, TileType.Quicksand);
        }
    }

    private void OnDestroy() {
        if (grid != null) {
            grid.OnValueChanged -= UpdateTileVisual;
        }
    }
}