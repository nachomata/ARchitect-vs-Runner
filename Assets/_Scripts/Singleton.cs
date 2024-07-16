using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Singleton : MonoBehaviour
{
    public static Singleton Instance;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            return;
        }

        Instance = this;
        
    }

    public GameObject keyTile;

    [Tooltip("Tiles must be in order: Start, End, Normal")]
    public GameObject[] otherTiles;
    public enum TileType { Key, Start, End, Normal };
    public int matrixSize = 7;
    public float tileSize = 2f, tileOffset = 0.5f;

    public struct Tile
    {
        public TileType type;
        public GameObject gameObject;
        public bool visible;
    }
    public Tile[] tiles;

    public Tile[,] gameMatrix;

    void StartTiles()
    {
        tiles[0] = new Tile { type = TileType.Key, gameObject = keyTile, visible = false };
        // tiles[1] = new Tile { type = TileType.Start, gameObject = otherTiles[0], visible = false };
        // tiles[2] = new Tile { type = TileType.End, gameObject = otherTiles[1], visible = false };
    }

    void Start()
    {
        StartTiles();
        Singleton.Instance.matrixSize = 9;
    }


}


