using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Scene References")]
    public Player player1;
    public Player player2;
    public Ball ball;
    public Sprite[] fieldSprites;
    public bool randomizeFieldSprite = false;

    [Header("Obstacles")]
    public GameObject[] obstaclePrefabs;
    public bool randomizeObstacles = false;
    public bool randomizeObstaclesOnRefresh = false;
    [Range(0, 10)]
    public int minObstacles = 2;
    [Range(0, 10)]
    public int maxObstacles = 5;
    public Transform obstaclesParent;

    [HideInInspector] public List<Player> players;

    [HideInInspector] public Dictionary<string, Vector2> locations;


    public float FieldHalfWidth = 9f;
    public float GoalHalfHeight = 2f;

    private GameObject _field;
    private List<GameObject> _spawnedObstacles = new List<GameObject>();

    void Awake()
    {
        // Find the field GameObject in the scene
        _field = GameObject.Find("Field");
        if (_field == null)
        {
            Debug.LogError("Field GameObject not found in the scene.");
        }

        // Create obstacles parent if not assigned
        if (obstaclesParent == null)
        {
            GameObject obstaclesObj = new GameObject("Obstacles");
            obstaclesParent = obstaclesObj.transform;
        }

        // Initialize helper collections for the behavior tree
        players = new List<Player> { player1, player2 };

        locations = new Dictionary<string, Vector2>
        {
            {"rtg", new Vector2(FieldHalfWidth,  GoalHalfHeight)},
            {"rbg", new Vector2(FieldHalfWidth, -GoalHalfHeight)},
            {"btg", new Vector2(-FieldHalfWidth,  GoalHalfHeight)},
            {"bbg", new Vector2(-FieldHalfWidth, -GoalHalfHeight)}
        };
    }

    void Start()
    {
        if (randomizeObstacles)
        {
            RandomizeObstacles();
        }
        Refresh();
    }

    public void Refresh()
    {
        if (randomizeFieldSprite && fieldSprites.Length > 0)
        {
            int randomIndex = Random.Range(0, fieldSprites.Length);
            _field.GetComponent<SpriteRenderer>().sprite = fieldSprites[randomIndex];
        }

        if (randomizeObstacles && randomizeObstaclesOnRefresh)
        {
            RandomizeObstacles();
        }

        ball.Refresh();
        player1.Refresh();
        player2.Refresh();
    }

    private void RandomizeObstacles()
    {
        // Clear existing obstacles
        foreach (GameObject obstacle in _spawnedObstacles)
        {
            if (obstacle != null)
                Destroy(obstacle);
        }
        _spawnedObstacles.Clear();

        if (!randomizeObstacles || obstaclePrefabs == null || obstaclePrefabs.Length == 0)
        {
            return;
        }

        // Get field bounds
        SpriteRenderer fieldRenderer = _field.GetComponent<SpriteRenderer>();
        Bounds fieldBounds = fieldRenderer.bounds;

        // Determine number of obstacles to spawn
        int obstacleCount = Random.Range(minObstacles, maxObstacles + 1);

        for (int i = 0; i < obstacleCount; i++)
        {
            // Select random obstacle prefab
            GameObject prefab = obstaclePrefabs[Random.Range(0, obstaclePrefabs.Length)];

            // Determine position (keeping away from spawn points and center)
            Vector3 position = new Vector3(
                Random.Range(fieldBounds.min.x + 1, fieldBounds.max.x - 1),
                Random.Range(fieldBounds.min.y + 1, fieldBounds.max.y - 1),
                0
            );

            // Don't spawn too close to players or ball spawn points
            if (Vector3.Distance(position, player1.SpawnPoint.position) < 2f ||
                Vector3.Distance(position, player2.SpawnPoint.position) < 2f ||
                Vector3.Distance(position, ball.SpawnPoint.position) < 2f)
            {
                i--; // Try again
                continue;
            }

            // Instantiate obstacle
            GameObject obstacle = Instantiate(prefab, position, Quaternion.identity, obstaclesParent);
            _spawnedObstacles.Add(obstacle);
        }
    }
}
