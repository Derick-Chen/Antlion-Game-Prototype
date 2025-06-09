using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Testing : MonoBehaviour {
    [Header("Grid Settings")]
    public float cellSize = 10f;
    public Vector3 originPosition = Vector3.zero;

    [Header("Tile Visuals")]
    public TileSpriteData sandTileVisual;
    public TileSpriteData rockTileVisual;
    public TileSpriteData quicksandTileVisual;

    [Header("Ant Settings")]
    public Sprite antFacingUp;
    public Sprite antFacingDown;
    public Sprite antFacingLeft;
    public Sprite antFacingRight;

    public TextAsset levelFile; // Drag your level1.txt here

    private Grid<TileType> grid;
    private TilemapSpriteUV tilemapVisual;

    void Start() {
        LoadLevelFromTextAsset(levelFile);
    }

    private void LoadLevelFromTextAsset(TextAsset levelData) {
        string[] lines = levelData.text.Trim().Split('\n');
        int height = lines.Length;
        int width = lines[0].Trim().Split(' ').Length;

        grid = new Grid<TileType>(width, height, cellSize, originPosition);
        tilemapVisual = gameObject.AddComponent<TilemapSpriteUV>();

        TileSpriteData[] visuals = new TileSpriteData[] {
            new TileSpriteData {
                tileType = TileType.Sand,
                sprite = sandTileVisual.sprite,
                color = sandTileVisual.color
            },
            new TileSpriteData {
                tileType = TileType.Rock,
                sprite = rockTileVisual.sprite,
                color = rockTileVisual.color
            },
            new TileSpriteData {
                tileType = TileType.Quicksand,
                sprite = quicksandTileVisual.sprite,
                color = quicksandTileVisual.color
            }
        };

        tilemapVisual.Initialize(grid, visuals);

        for (int y = 0; y < height; y++) {
            string[] tokens = lines[y].Trim().Split(' ');
            for (int x = 0; x < width; x++) {
                grid.SetValue(x, y, CharToTileType(tokens[x]));
            }
        }

        CenterCameraOnGrid(width, height);
        SpawnAnt(new Vector2Int(0, 0)); // Start ant at tile (0,0)
    }

    private TileType CharToTileType(string c) {
        switch (c) {
            case "S": return TileType.Sand;
            case "R": return TileType.Rock;
            case "Q": return TileType.Quicksand;
            default: return TileType.Sand;
        }
    }

    private void CenterCameraOnGrid(int width, int height) {
        Vector3 center = new Vector3(
            width * cellSize / 2f + originPosition.x,
            height * cellSize / 2f + originPosition.y,
            Camera.main.transform.position.z
        );
        Camera.main.transform.position = center;
    }

    private void SpawnAnt(Vector2Int startGridPos) {
        GameObject antGO = new GameObject("Ant");
        SpriteRenderer renderer = antGO.AddComponent<SpriteRenderer>();
        renderer.sprite = antFacingUp; // Set initial facing direction
        renderer.sortingOrder = 10;

        AntController ant = antGO.AddComponent<AntController>();
        ant.antSprite = renderer;
        ant.antFacingUp = antFacingUp;
        ant.antFacingDown = antFacingDown;
        ant.antFacingLeft = antFacingLeft;
        ant.antFacingRight = antFacingRight;

        ant.Initialize(grid, tilemapVisual, startGridPos, cellSize);

    }
}