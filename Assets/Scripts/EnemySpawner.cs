using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WaveData
{
    public int Count;
}

public class EnemySpawner : MonoBehaviour
{
    [Header("敵プレハブ")]
    public GameObject enemyPrefab;

    [Header("スポーン位置")]
    public float spawnX = 0f;
    public float spawnY = 0f;
    public float spawnZ = 0f;

    [Header("Waveデータ")]
    public List<WaveData> waves = new List<WaveData>();

    [Header("スポーン設定")]
    public float spawnInterval = 1f;
    private float spawnTimer = 0f;

    private int currentWave = 0;
    private int spawnedOrc = 0;

    private float waveTimer = 0f;
    public float waveInterval = 20f;

    private bool waveActive = true;

    [Header("Waypoint 親オブジェクト")]
    public Transform waypointParent; // ← Waypoints の親オブジェクトを指定

    private Transform[] waypoints;


    void Start()
    {
        // Waypoints を配列として取得
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
        spawnTimer += Time.deltaTime;

        if (waveActive && spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }

        if (waveTimer >= waveInterval)
        {
            currentWave++;
            if (currentWave < waves.Count)
            {
                waveTimer = 0f;
                ResetSpawnCount();
                waveActive = true;
                Debug.Log("Wave " + (currentWave + 1) + " 開始！");
            }
            else
            {
                Debug.Log("全てのWaveが終了しました");
            }
        }
    }

    void SpawnEnemy()
    {
        if (currentWave >= waves.Count) return;

        WaveData wave = waves[currentWave];

        if (spawnedOrc < wave.Count)
        {
            GameObject enemy = Instantiate(enemyPrefab, new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);

            // EnemyPath に waypoint を渡す
            EnemyPath path = enemy.GetComponent<EnemyPath>();
            if (path != null)
            {
                path.SetWaypoints(waypoints);
            }

            spawnedOrc++;
        }
        else
        {
            waveActive = false;
        }
    }

    void ResetSpawnCount()
    {
        spawnedOrc = 0;
    }
}
