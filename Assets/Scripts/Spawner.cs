using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// データ構造のクラスは変更なし
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
    private bool waveActive = true;

    private GameManager gameManager;
    private int enemiesRemainingInFinalWave = 0;
    private bool isFinalWaveActive = false;
    private const int FINAL_WAVE_NUMBER = 5; // Wave5でクリア

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
            ActivateFinalWaveCheck();
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
                    GameObject enemyGO = Instantiate(enemyData.enemyPrefab, new Vector3(spawnX, spawnY, spawnZ), Quaternion.identity);
                    
                    EnemyHealth health = enemyGO.GetComponent<EnemyHealth>();
                    if (health != null)
                    {
                        health.waveOrigin = currentWave;
                    }

                    EnemyPath path = enemyGO.GetComponent<EnemyPath>();
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

        // ▼▼▼ この行を修正 ▼▼▼
        // ★ 最終ウェーブの監視が始まっていない場合のみ、時間経過で次のウェーブに進む
        if (waveTimer >= waveInterval && !isFinalWaveActive)
        {
            currentWave++;
            if (currentWave < waves.Count)
            {
                Debug.Log($"Wave {currentWave + 1} 開始！");
                ResetWave();

                if (currentWave + 1 == FINAL_WAVE_NUMBER)
                {
                    ActivateFinalWaveCheck();
                }
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

    void ActivateFinalWaveCheck()
    {
        isFinalWaveActive = true;
        enemiesRemainingInFinalWave = 0;
        foreach (var enemyData in waves[currentWave].enemies)
        {
            enemiesRemainingInFinalWave += enemyData.count;
        }
        Debug.Log($"最終ウェーブ開始！クリア監視を始めます。倒すべき敵の数: {enemiesRemainingInFinalWave}");
    }
    
    public void OnEnemyDefeated(int defeatedWaveOrigin)
    {
        if (isFinalWaveActive && defeatedWaveOrigin == currentWave)
        {
            enemiesRemainingInFinalWave--;
            Debug.Log($"最終ウェーブの敵を撃破。残り: {enemiesRemainingInFinalWave}");

            if (enemiesRemainingInFinalWave <= 0)
            {
                isFinalWaveActive = false;
                Debug.Log("最終ウェーブの敵を全滅！ゲームクリア！");
                if (gameManager != null)
                {
                    gameManager.GameClear();
                }
            }
        }
    }
}