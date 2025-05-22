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

    [Header("Waveデータ")]
    public List<WaveData> waves = new List<WaveData>();

    [Header("スポーン設定")]
    public float spawnInterval = 1f;
    private float spawnTimer = 0f;

    private int currentWave = 0;
    private int spawnedOrc = 0;
    private int spawnedSoldier = 0;
    private int spawnedLancer = 0;

    private float waveTimer = 0f;
    public float waveInterval = 20f; // 次のWaveに進むまでの時間

    private bool waveActive = true;

    void Update()
    {
        if (currentWave >= waves.Count) return; // 全Wave終了

        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        if (waveActive && spawnTimer >= spawnInterval)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }

        // Wave切り替え
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
            Instantiate(enemyPrefab, new Vector3(spawnX, spawnY, 0f), Quaternion.identity);
            spawnedOrc++;
        }
        else
        {
            waveActive = false; // 全部出し終えた
        }
    }

    void ResetSpawnCount()
    {
        spawnedOrc = 0;
        spawnedSoldier = 0;
        spawnedLancer = 0;
    }
}
