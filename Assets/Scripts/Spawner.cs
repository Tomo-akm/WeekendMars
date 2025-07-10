using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveEnemyData
{
    public GameObject enemyPrefab;
    public int count;
    public float spawnInterval;
    [System.NonSerialized] public int spawned = 0;
    [System.NonSerialized] public float timer = 0f;
}

[System.Serializable]
public class CustomWaveData
{
    public List<WaveEnemyData> enemies = new List<WaveEnemyData>();
}

public class Spawner : MonoBehaviour
{
    [Header("Waveデータ")]
    public List<CustomWaveData> waves = new List<CustomWaveData>();
    [Header("スポーン位置")]
    public float spawnX = 10f;
    public float spawnY = -3.3f;
    public float spawnZ = 0f;

    [Header("Wave間隔")]
    public float waveInterval = 20f;
    private float waveTimer = 0f;

    [Header("Waypoint 親オブジェクト")]
    public Transform waypointParent;
    private Transform[] waypoints;

    private int currentWave = 0;
    private int currentEnemyType = 0;
    private int spawnedCount = 0;
    private float spawnTimer = 0f;
    private bool waveActive = true;

    void Start()
    {
        int count = waypointParent.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }
    }

    void Update()
    {
        if (currentWave >= waves.Count) return;

        waveTimer += Time.deltaTime;

        if (waveActive)
        {
            var wave = waves[currentWave];
            bool allEnemiesSpawned = true;

            foreach (var enemyData in wave.enemies)
            {
                if (enemyData.spawned >= enemyData.count) continue;

                allEnemiesSpawned = false;
                enemyData.timer += Time.deltaTime;

                if (enemyData.timer >= enemyData.spawnInterval)
                {
                    GameObject enemy = Instantiate(enemyData.enemyPrefab, new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
                    EnemyPath path = enemy.GetComponent<EnemyPath>();
                    if (path != null)
                        path.SetWaypoints(waypoints);

                    enemyData.spawned++;
                    enemyData.timer = 0f;
                }
            }

            if (allEnemiesSpawned)
            {
                waveActive = false;
            }
        }

        if (waveTimer >= waveInterval)
        {
            currentWave++;
            if (currentWave < waves.Count)
            {
                Debug.Log($"Wave {currentWave + 1} 開始！");
                ResetWave();
            }
            else
            {
                Debug.Log("全てのWaveが終了しました");
            }
        }
    }


    void ResetWave()
    {
        waveTimer = 0f;
        waveActive = true;

        var wave = waves[currentWave];
        foreach (var enemy in wave.enemies)
        {
            enemy.spawned = 0;
            enemy.timer = 0f;
        }
    }
}
