using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Grid Positions (Size = 9)")]
    public Transform[] gridPositions;

    [Header("Tile Prefabs (Size = 4)")]
    public GameObject[] tilePrefabs;

    [Header("Round Settings")]
    public float maxRoundTime = 5f;
    public float fadeDuration = 5f;

    float roundTimer;
    bool roundActive;

    List<GameObject> spawnedTiles = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        StartRound();
    }

    void Update()
    {
        if (roundActive)
        {
            roundTimer += Time.deltaTime;
        }
    }

    void StartRound()
    {
        if (ScoreManager.Instance.isGameOver)
            return;

        roundActive = true;
        roundTimer = 0f;

        // Pick odd position
        int winIndex = Random.Range(0, gridPositions.Length);

        // Pick odd & normal prefabs (must be different)
        int oddPrefabIndex = Random.Range(0, tilePrefabs.Length);
        int normalPrefabIndex;

        do
        {
            normalPrefabIndex = Random.Range(0, tilePrefabs.Length);
        }
        while (normalPrefabIndex == oddPrefabIndex);

        GameObject oddPrefab = tilePrefabs[oddPrefabIndex];
        GameObject normalPrefab = tilePrefabs[normalPrefabIndex];

        // Spawn tiles
        for (int i = 0; i < gridPositions.Length; i++)
        {
            GameObject prefabToSpawn = (i == winIndex) ? oddPrefab : normalPrefab;

            GameObject tile = Instantiate(
                prefabToSpawn,
                gridPositions[i].position,
                prefabToSpawn.transform.rotation   // preserve prefab rotation
            );

            TileController tc = tile.GetComponent<TileController>();
            tc.Initialize(i == winIndex, fadeDuration);

            spawnedTiles.Add(tile);
        }
    }

    public void OnTileClicked(bool isOdd)
    {
        if (!roundActive)
            return;

        roundActive = false;

        if (isOdd)
        {
            int scoreToAdd = CalculateScoreFromTime(roundTimer);
            ScoreManager.Instance.AddScore(scoreToAdd);
        }
        else
        {
            ScoreManager.Instance.ApplyWrongClickPenalty();
        }

        // Stop spawning if game over
        if (ScoreManager.Instance.isGameOver)
            return;

        Invoke(nameof(StartNextRound), 0.1f);
    }

    int CalculateScoreFromTime(float timeTaken)
    {
        if (timeTaken <= 1f)
            return 100;
        else if (timeTaken <= 2f)
            return 80;
        else if (timeTaken <= 3f)
            return 60;
        else if (timeTaken <= 4f)
            return 40;
        else if (timeTaken <= maxRoundTime)
            return 20;
        else
            return 0;
    }

    void StartNextRound()
    {
        foreach (GameObject tile in spawnedTiles)
        {
            Destroy(tile);
        }

        spawnedTiles.Clear();
        StartRound();
    }
}
