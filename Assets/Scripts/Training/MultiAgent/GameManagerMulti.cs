using System.Collections.Generic;
using EasyTextEffects.Editor.MyBoxCopy.Extensions;
using UnityEngine;

public class GameManagerMulti : MonoBehaviour
{
    [Header("Scene References")]
    public TeamGroup LeftTeam;
    public TeamGroup RightTeam;
    public int TimeScale;
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
        if (obstaclesParent == null && randomizeObstacles)
        {
            GameObject obstaclesObj = new GameObject("Obstacles");
            obstaclesParent = obstaclesObj.transform;
        }
    }

    void Start()
    {
        if (randomizeObstacles)
        {
            RandomizeObstacles();
        }
        if (TimeScale > 0)
        {
            Time.timeScale = TimeScale;
        }
        else
        {
            Debug.LogWarning("TimeScale is set to 0 or negative, game time will not progress.");
        }
        Refresh();
    }

    public void Refresh(bool random = false)
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
        LeftTeam.Refresh();
        RightTeam.Refresh();
        if (random)
        {
            for (int i = 0; i < LeftTeam.Players.Length; i++)
            {
                LeftTeam.Players[i].SetVelocity(GetRandomVelocity(LeftTeam.Players[i].NormalSpeed));
                RightTeam.Players[i].SetVelocity(GetRandomVelocity(RightTeam.Players[i].NormalSpeed));
            }
            ball.SetVelocity(GetRandomVelocity(LeftTeam.Players[0].KickPower));
        }
    }
    public Vector2 GetRandomVelocity(float maxSpeed)
    {
        Vector2 dir = Random.insideUnitCircle.normalized;

        return dir * Random.Range(0f, maxSpeed);
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
            for (int j = 0; j < LeftTeam.Players.Length; j++)
            {
                if (Vector3.Distance(position, LeftTeam.Players[j].SpawnPoint.position) < 2f ||
                    Vector3.Distance(position, RightTeam.Players[j].SpawnPoint.position) < 2f)
                {
                    i--; // Try again
                    continue;
                }
            }
            for (int j = 0; j < RightTeam.Players.Length; j++)
            {
                if (Vector3.Distance(position, RightTeam.Players[j].SpawnPoint.position) < 2f)
                {
                    i--; // Try again
                    continue;
                }
            }

            // Instantiate obstacle
            GameObject obstacle = Instantiate(prefab, position, Quaternion.identity, obstaclesParent);
            _spawnedObstacles.Add(obstacle);
        }
    }
}
