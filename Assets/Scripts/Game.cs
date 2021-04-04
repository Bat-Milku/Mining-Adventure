using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public int gridWidth = 24;
    public int gridHeight = 16;
    public TileType[] room = null; 
    public Sprite[] TileImages;
    public GameObject tilePrefab;
    private GameObject[] cells = null;
    public float numRocks = .1f;
    public float numGems = .05f;
    public Transform Room;
    public Controller player;

    // Start is called before the first frame update
    void Start()
    {
        room = new TileType[gridWidth * gridHeight];
        cells = new GameObject[gridWidth * gridHeight];
        Generate();
    }

    void Generate()
    {
        foreach (Transform t in Room)
            Destroy(t.gameObject);

        // Generate the walls

        for (int y = 0; y < gridHeight; y++)
        {
            room[0 + gridWidth * y] = TileType.Wall;
            room[(gridWidth - 1) + gridWidth * y] = TileType.Wall;
        }
        for (int x = 0; x < gridWidth; x++)
        {
            room[x + gridWidth * (gridHeight - 1)] = TileType.Wall;
        }
        for (int x = 1; x < gridWidth - 1; x++)
        {
            for (int y = 0; y < gridHeight - 1; y++)
            {
                room[x + gridWidth * y] = TileType.Dirt;
            }
        }

        // Generate Rocks
        int num = (int)((gridWidth - 2) * gridHeight * numRocks);
        for (int i = 0; i < num; i++)
        {
            int x = Random.Range(1, gridWidth - 1);
            int y = Random.Range(0, gridHeight - 1);
            room[x + gridWidth * y] = TileType.Rock;
        }

        // Generate gems

        num = (int)((gridWidth - 2) * gridHeight * numGems);
        for (int i = 0; i < num; i++)
        {
            int x = Random.Range(1, gridWidth - 1);
            int y = Random.Range(0, gridHeight - 1);
            room[x + gridWidth * y] = TileType.Gem;
        }

        // Draw the room

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                int pos = x + gridWidth * y;
                GameObject tile = Instantiate(tilePrefab, Room);
                cells[pos] = tile;
                tile.GetComponent<SpriteRenderer>().sprite = TileImages[(int)room[pos]];
                tile.transform.position = new Vector3(x, -y, 0);
                byte col = (byte)(255 - y * 12);
                if (x > 0 && x < gridWidth - 1 && y < gridHeight - 1) tile.GetComponent<SpriteRenderer>().color = new Color32(col, col, col, 255);
            }
        }
        

        player.transform.position = new Vector3(gridWidth / 2, 2, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Escape)) {
            Generate();
        }
    }
    public TileType Dig(int x, int y)
    {
        if (x < 0 || x >= gridWidth || y < 0 || y >= gridHeight) return TileType.Empty;

        TileType res = room[x + gridWidth * y];
        if (res == TileType.Empty) return TileType.Empty;
        room[x + gridWidth * y] = TileType.Empty;
        Destroy(cells[x + gridWidth * y]);
        cells[x + gridWidth * y] = null;
        return res;
    }
}
public enum TileType
{
    Empty = 0,
    Wall = 1,
    Rock = 2,
    Gem = 3,
    Dirt = 4,
}