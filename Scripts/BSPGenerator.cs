using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.AI;
using NavMeshPlus.Components;
using System.Collections.Generic;

/// <summary>
/// Generates a random dungeon layout using the Binary Space Partitioning (BSP) algorithm.
/// This script creates rooms and corridors, populates Tilemaps with floor and wall tiles,
/// places spawn points for the player, goal, researcher, and enemies, and bakes a NavMesh for AI navigation.
/// </summary>
public class BSPGenerator : MonoBehaviour
{
    public int mapWidth = 64, mapHeight = 64;
    public int minRoomSize = 6, maxDepth = 4;
    
    public Tilemap floorTilemap;
    public Tilemap wallTilemap;
    public List<TileBase> floorTileList;
    public TileBase wallTile;

    private GameObject playerSpawn;
    public GameObject goalSpawnPrefab;
    public GameObject researcherSpawnPrefab;
    public GameObject enemySpawnPrefab;
    
    private BSPNode root;
    public List<Rect> rooms = new List<Rect>();
    public List<Vector2Int> corridors = new List<Vector2Int>();

    public GameObject navMeshGO;
    private NavMeshSurface navMeshSurface;

    /// <summary>
    ///  Initializes the dungeon generation process when the script starts.
    /// </summary>
    void Start()
    {
        // Initialize the root node of the BSP tree, representing the entire map area.
        root = new BSPNode(new Rect(0, 0, mapWidth, mapHeight));

        // Recursively split the map using the BSP algorithm.
        Split(root, maxDepth);

        // Generate rooms within the partitioned areas of the BSP tree.
        GenerateRooms(root);

        // Connect the generated rooms with corridors.
        ConnectRooms();

        //  Generate the Tilemap based on the room and corridor layouts.
        GenerateTilemap();

        // Find the player's spawn point using its tag.  Ensure the Player is tagged.
        playerSpawn = GameObject.FindWithTag("Player");
        PlaceSpawnPoints();

        // Get the NavMeshSurface component from the specified GameObject.
        navMeshSurface = navMeshGO.GetComponent<NavMeshSurface>();
        BakeNavMesh(); // Generate the NavMesh.
    }

    /// <summary>
    ///  Bakes (generates) the NavMesh, allowing AI characters to navigate the dungeon.
    /// </summary>
    void BakeNavMesh() 
    {
        navMeshSurface.BuildNavMesh();
    }

    /// <summary>
    /// Recursively splits a rectangular area using the Binary Space Partitioning algorithm.
    /// </summary>
    /// <param name="node">The current BSP node representing the area to split.</param>
    /// <param name="depth">The current recursion depth.  Controls how many times the area is split.</param>
    void Split(BSPNode node, int depth)
    {
        // Base case: Stop splitting if maximum depth is reached or the area is too small.
        if (depth <= 0 || node.Area.width < minRoomSize * 2 || node.Area.height < minRoomSize * 2)
            return;

        // Determine whether to split horizontally or vertically (biased towards the larger dimension).
        bool splitH = Random.value > 0.5f;
        if (node.Area.width > node.Area.height) splitH = false;
        else if (node.Area.height > node.Area.width) splitH = true;

        // Calculate the split point, ensuring the resulting areas are not too small.
        float splitPoint = splitH
            ? Random.Range(minRoomSize, node.Area.height - minRoomSize)
            : Random.Range(minRoomSize, node.Area.width - minRoomSize);

        // Create the left and right child nodes based on the split orientation.
        if (splitH)
        {
            node.Left = new BSPNode(new Rect(node.Area.x, node.Area.y, node.Area.width, splitPoint));
            node.Right = new BSPNode(new Rect(node.Area.x, node.Area.y + splitPoint, node.Area.width, node.Area.height - splitPoint));
        }
        else
        {
            node.Left = new BSPNode(new Rect(node.Area.x, node.Area.y, splitPoint, node.Area.height));
            node.Right = new BSPNode(new Rect(node.Area.x + splitPoint, node.Area.y, node.Area.width - splitPoint, node.Area.height));
        }

        // Recursively split the child nodes.
        Split(node.Left, depth - 1);
        Split(node.Right, depth - 1);
    }

    /// <summary>
    ///  Generates rooms within the leaf nodes of the BSP tree.
    /// </summary>
    /// <param name="node">The current BSP node.</param>
    void GenerateRooms(BSPNode node)
    {
        // If the node has children, process them recursively.
        if (node.Left != null && node.Right != null)
        {
            GenerateRooms(node.Left);
            GenerateRooms(node.Right);
        }
        // If the node is a leaf node (no children), create a room.
        else
        {
            //  Calculate random room dimensions and position within the node's area.
            float roomWidth = Random.Range(minRoomSize, node.Area.width - 2);
            float roomHeight = Random.Range(minRoomSize, node.Area.height - 2);
            float roomX = Random.Range(node.Area.x + 1, node.Area.x + node.Area.width - roomWidth - 1);
            float roomY = Random.Range(node.Area.y + 1, node.Area.y + node.Area.height - roomHeight - 1);

            // Store the room's rectangle in the node and add it to the rooms list.
            node.Room = new Rect(roomX, roomY, roomWidth, roomHeight);
            rooms.Add(node.Room);
        }
    }

    /// <summary>
    /// Connects the generated rooms with corridors.  Connects each room to the next room in the list.
    /// </summary>
    void ConnectRooms()
    {
        for (int i = 0; i < rooms.Count - 1; i++)
        {
            // Get the centers of two adjacent rooms.
            Vector2Int start = new Vector2Int((int)rooms[i].center.x, (int)rooms[i].center.y);
            Vector2Int end = new Vector2Int((int)rooms[i + 1].center.x, (int)rooms[i + 1].center.y);

            // Create a corridor connecting the two rooms.
            CreateCorridor(start, end);
        }
    }

    /// <summary>
    /// Creates a corridor between two points using an L-shaped path.
    /// </summary>
    /// <param name="start">The starting point of the corridor.</param>
    /// <param name="end">The ending point of the corridor.</param>
    void CreateCorridor(Vector2Int start, Vector2Int end)
    {
        // Randomly choose the order of horizontal and vertical segments.
        if (Random.value > 0.5f)
        {
            // Create horizontal segment first, then vertical.
            CreateWideHorizontalCorridor(start.x, end.x, start.y);
            CreateWideVerticalCorridor(start.y, end.y, end.x);
        }
        else
        {
            // Create vertical segment first, then horizontal.
            CreateWideVerticalCorridor(start.y, end.y, start.x);
            CreateWideHorizontalCorridor(start.x, end.x, end.y);
        }
    }

    /// <summary>
    /// Creates a horizontal corridor with a width of 3 tiles.
    /// </summary>
    /// <param name="x1">The starting x-coordinate.</param>
    /// <param name="x2">The ending x-coordinate.</param>
    /// <param name="y">The y-coordinate of the corridor.</param>
    void CreateWideHorizontalCorridor(int x1, int x2, int y)
    {
        for (int x = Mathf.Min(x1, x2); x <= Mathf.Max(x1, x2); x++)
        {
            for (int offset = -1; offset <= 1; offset++) // 3 tiles wide
            {
                corridors.Add(new Vector2Int(x, y + offset));
            }
        }
    }

    /// <summary>
    /// Creates a vertical corridor with a width of 3 tiles.
    /// </summary>
    /// <param name="y1">The starting y-coordinate.</param>
    /// <param name="y2">The ending y-coordinate.</param>
    /// <param name="x">The x-coordinate of the corridor.</param>
    void CreateWideVerticalCorridor(int y1, int y2, int x)
    {
        for (int y = Mathf.Min(y1, y2); y <= Mathf.Max(y1, y2); y++)
        {
            for (int offset = -1; offset <= 1; offset++) // 3 tiles wide
            {
                corridors.Add(new Vector2Int(x + offset, y));
            }
        }
    }

    /// <summary>
    ///  Generates the Tilemap based on the generated rooms and corridors.
    /// </summary>
    void GenerateTilemap()
    {
        // Clear any existing tiles.
        floorTilemap.ClearAllTiles();
        wallTilemap.ClearAllTiles();
        HashSet<Vector2Int> floorTiles = new HashSet<Vector2Int>(); // Use HashSet for efficiency

        // Place floor tiles for each corridor.
        foreach (Vector2Int corridor in corridors)
        {
            floorTilemap.SetTile(new Vector3Int(corridor.x, corridor.y, 0), floorTileList[0]);
            floorTiles.Add(corridor);
        }

        // Place floor tiles for each room.
        foreach (Rect room in rooms)
        {
            // Select a random floor tile from list
            int floorIndex = Random.Range(1, floorTileList.Count - 1);

            for (int x = (int)room.x; x < (int)(room.x + room.width); x++)
            {
                for (int y = (int)room.y; y < (int)(room.y + room.height); y++)
                {
                    Vector3Int tilePos = new Vector3Int(x, y, 0);
                    floorTilemap.SetTile(tilePos, floorTileList[floorIndex]);
                    floorTiles.Add(new Vector2Int(x, y));
                }
            }
        }

        // Determine where walls should be placed. Walls are placed next to floor tiles.
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();
        foreach (Vector2Int floorPos in floorTiles)
        {
            Vector2Int[] adjacentPositions =
            {
                new Vector2Int(floorPos.x + 1, floorPos.y), // Right
                new Vector2Int(floorPos.x - 1, floorPos.y), // Left
                new Vector2Int(floorPos.x, floorPos.y + 1), // Up
                new Vector2Int(floorPos.x, floorPos.y - 1)  // Down
            };

            // Check adjacent positions for walls
            foreach (Vector2Int adjacent in adjacentPositions)
            {
                if (!floorTiles.Contains(adjacent))
                {
                    wallPositions.Add(adjacent);
                }
            }

            // Check diagonal positions for corner tiles
            Vector2Int[] diagonalPositions =
            {
                new Vector2Int(floorPos.x + 1, floorPos.y + 1), // Top-right corner
                new Vector2Int(floorPos.x - 1, floorPos.y + 1), // Top-left corner
                new Vector2Int(floorPos.x + 1, floorPos.y - 1), // Bottom-right corner
                new Vector2Int(floorPos.x - 1, floorPos.y - 1)  // Bottom-left corner
            };

            // Check if both adjacent walls exist to form a corner
            foreach (Vector2Int diagonal in diagonalPositions)
            {
                if (wallPositions.Contains(new Vector2Int(floorPos.x + 1, floorPos.y)) &&
                    wallPositions.Contains(new Vector2Int(floorPos.x, floorPos.y + 1)) ||
                    wallPositions.Contains(new Vector2Int(floorPos.x - 1, floorPos.y)) &&
                    wallPositions.Contains(new Vector2Int(floorPos.x, floorPos.y + 1)) ||
                    wallPositions.Contains(new Vector2Int(floorPos.x + 1, floorPos.y)) &&
                    wallPositions.Contains(new Vector2Int(floorPos.x, floorPos.y - 1)) ||
                    wallPositions.Contains(new Vector2Int(floorPos.x - 1, floorPos.y)) &&
                    wallPositions.Contains(new Vector2Int(floorPos.x, floorPos.y - 1)))
                {
                    if (!floorTiles.Contains(diagonal)){
                        wallPositions.Add(diagonal);
                    }
                }
            }
        }

        // Place wall tiles (including corner tiles).
        foreach (Vector2Int wallPos in wallPositions)
        {
            wallTilemap.SetTile(new Vector3Int(wallPos.x, wallPos.y, 0), wallTile);
        }

    }

    /// <summary>
    /// Places the spawn points for the player, goal, researcher, and enemies.
    /// </summary>
    void PlaceSpawnPoints()
    {
        if (rooms.Count == 0) return;

        // Place player at the center of the first room.
        Vector2 playerPos = rooms[0].center;
        playerSpawn.transform.position = new Vector3(playerPos.x, playerPos.y, 0);
        //Instantiate(playerSpawnPrefab, new Vector3(playerPos.x, playerPos.y, 0), Quaternion.identity); // commented out, assuming player exists.

        // Place goal at the center of the last room.
        Vector2 goalPos = rooms[rooms.Count - 1].center;
        Instantiate(goalSpawnPrefab, new Vector3(goalPos.x, goalPos.y, 0), Quaternion.identity);

        // Place researcher in a random room (excluding first and last).
        int researcherRoomIndex = Random.Range(1, rooms.Count - 1);
        Vector2 researcherPos = rooms[researcherRoomIndex].center;
        Instantiate(researcherSpawnPrefab, new Vector3(researcherPos.x, researcherPos.y, 0), Quaternion.identity);

        // Place enemies in the remaining rooms, avoiding the researcher's room.
        for (int i = 1; i < rooms.Count - 1; i++)
        {
            if (i == researcherRoomIndex) continue; // Avoid placing an enemy in the researcher's room.

            Vector2 enemyPos = rooms[i].center;
            Instantiate(enemySpawnPrefab, new Vector3(enemyPos.x, enemyPos.y, 0), Quaternion.identity);
        }
    }
}
