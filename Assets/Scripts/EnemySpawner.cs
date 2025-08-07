// EnemySpawner.cs (完全版)

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
    public Transform waypointParent;

    private Transform[] waypoints;

    // ▼▼▼ おそらく、この部分の変数が抜けています ▼▼▼
    private GameManager gameManager;
    private int enemiesRemainingInFinalWave = 0;
    private bool isFinalWaveActive = false;
    private const int FINAL_WAVE_NUMBER = 4;
    // ▲▲▲ ここまで ▲▲▲

    void Start()
    {
        gameManager = GameManager.instance;

        int count = waypointParent.childCount;
        waypoints = new Transform[count];
        for (int i = 0; i < count; i++)
        {
            waypoints[i] = waypointParent.GetChild(i);
        }

        if (waves.Count > 0 && FINAL_WAVE_NUMBER == 1)
        {
            isFinalWaveActive = true;
            enemiesRemainingInFinalWave = waves[0].Count;
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

                if (currentWave + 1 == FINAL_WAVE_NUMBER)
                {
                    isFinalWaveActive = true;
                    enemiesRemainingInFinalWave = waves[currentWave].Count; 
                    Debug.Log("最終ウェーブ開始！クリア監視を始めます。");
                }
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

    public void OnEnemyDefeated()
    {
        if (isFinalWaveActive)
        {
            enemiesRemainingInFinalWave--;
            Debug.Log("最終ウェーブの敵を撃破。残り: " + enemiesRemainingInFinalWave);

            if (enemiesRemainingInFinalWave <= 0)
            {
                isFinalWaveActive = false; 
                Debug.Log("最終ウェーブの敵を全滅！ゲームクリア！");
                gameManager.GameClear();
            }
        }
    }
}