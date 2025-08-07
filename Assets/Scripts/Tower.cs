using UnityEngine;

public class Tower : MonoBehaviour
{
    [Header("射撃設定")]
    public GameObject shotPrefab;        // 弾のプレハブ
    public Transform firePoint;         // 発射地点
    public float attackRate = 1f;       // 攻撃頻度（秒間攻撃回数）
    public float detectionRange = 10f;  // 検出範囲
    public float shotSpeed = 12f;       // 弾の速度（敵より速く）
    public float damage = 10f;          // ダメージ値（追加）
    
    [Header("デバッグ")]
    public bool showDebugInfo = true;   // デバッグ情報表示
    
    private float nextFireTime = 0f;    // 次回発射可能時間
    private TowerUpgrade upgradeComponent; // アップグレードコンポーネント（追加）
    
    private void Start()
    {
        // アップグレードコンポーネントを取得
        upgradeComponent = GetComponent<TowerUpgrade>();
        
        Debug.Log("=== Tower初期化 ===");
        Debug.Log($"shotPrefab設定: {(shotPrefab != null ? shotPrefab.name : "未設定")}");
        Debug.Log($"検出範囲: {detectionRange}");

        if (firePoint == null)
        {
            firePoint = transform;
            Debug.Log("firePointが未設定のため、タワー自身を使用します");
        }
    }
    
    private void Update()
    {
        if (Time.time >= nextFireTime)
        {
            GameObject nearestEnemy = FindNearestEnemy();

            if (nearestEnemy != null)
            {
                // 予測射撃で弾を発射
                bool fireSuccess = FirePredictiveShot(nearestEnemy);

                if (fireSuccess)
                {
                    nextFireTime = Time.time + (1f / attackRate);
                    Debug.Log($"予測射撃成功: 目標={nearestEnemy.name}");
                }
                // 効果音を再生
                if (SEPlayer.instance != null)
                {
                    SEPlayer.instance.PlayBulletSE();
                }
            }
        }
    }
    
    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0)
            return null;

        GameObject bestTarget = null;
        float nearestDistance = detectionRange;
        int maxWaypointIndex = -1;
        bool foundInRange = false;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance > detectionRange)
                continue; // 射程外は無視

            EnemyPath path = enemy.GetComponent<EnemyPath>();
            if (path == null)
                continue;

            foundInRange = true;
            // より進んでいる敵を優先。進行度が同じ場合は距離が近い方を優先
            if (path.currentWaypointIndex > maxWaypointIndex ||
                (path.currentWaypointIndex == maxWaypointIndex && distance < nearestDistance))
            {
                maxWaypointIndex = path.currentWaypointIndex;
                nearestDistance = distance;
                bestTarget = enemy;
            }
        }
        // 射程内の敵がいなければnullを返す
        if (!foundInRange)
            return null;
        return bestTarget;
    }
    
    /// <summary>
    /// 予測射撃で弾を発射
    /// </summary>
    private bool FirePredictiveShot(GameObject targetEnemy)
    {
        if (shotPrefab == null || firePoint == null || targetEnemy == null)
            return false;
        
        // 敵の情報を取得
        Vector2 enemyPosition = targetEnemy.transform.position;
        Vector2 firePosition = firePoint.position;

        // 敵の進行方向と速度を取得
        EnemyPath path = targetEnemy.GetComponent<EnemyPath>();
        Vector2 enemyDirection = path != null ? path.LastMoveDirection : Vector2.left;
        float enemySpeed = path != null ? path.speed : 3f;
        Vector2 enemyVelocity = enemyDirection.normalized * enemySpeed;

        // 迎撃点計算
        Vector2 toEnemy = enemyPosition - firePosition;
        float a = Vector2.Dot(enemyVelocity, enemyVelocity) - shotSpeed * shotSpeed;
        float b = 2 * Vector2.Dot(toEnemy, enemyVelocity);
        float c = Vector2.Dot(toEnemy, toEnemy);
        float discriminant = b * b - 4 * a * c;
        float t = 0f;
        if (Mathf.Abs(a) < 0.0001f) {
            // aが0に近い場合は直線的に計算
            t = c / Mathf.Max(-b, 0.0001f);
        } else if (discriminant >= 0) {
            float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
            float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
            t = Mathf.Max(t1, t2, 0f);
        }
        t = Mathf.Max(t, 0.05f); // 負や極小値を防ぐ

        Vector2 predictedPosition = enemyPosition + enemyVelocity * t;
        Vector2 fireDirection = (predictedPosition - firePosition).normalized;

        // 弾を生成
        GameObject shotObject = Instantiate(shotPrefab, firePosition, Quaternion.identity);
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        shotObject.transform.rotation = Quaternion.Euler(0, 0, angle);

        float shotDamage = damage;
        if (upgradeComponent != null)
        {
            shotDamage = upgradeComponent.GetCurrentDamage();
        }
        PredictiveShot shot = shotObject.GetComponent<PredictiveShot>();
        if (shot == null)
        {
            shot = shotObject.AddComponent<PredictiveShot>();
        }
        shot.Initialize(fireDirection, shotSpeed, shotDamage);

        if (showDebugInfo)
        {
            Debug.DrawLine(firePosition, predictedPosition, Color.green, 1f);
            Debug.DrawRay(firePosition, fireDirection * 5f, Color.red, 1f);
        }
        return true;
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        if (firePoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(firePoint.position, 0.2f);
        }
    }
}
