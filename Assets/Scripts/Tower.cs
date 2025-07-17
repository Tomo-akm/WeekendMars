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
        
        GameObject nearest = null;
        float nearestDistance = detectionRange;
        
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearest = enemy;
                nearestDistance = distance;
            }
        }
        
        return nearest;
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
        
        // 敵の速度を推定（左に移動していると仮定）
        float enemySpeed = 3f;
        Vector2 enemyVelocity = new Vector2(-enemySpeed, 0f);
        
        // 弾が敵に到達する時間を計算
        Vector2 toEnemy = enemyPosition - firePosition;
        float distanceToEnemy = toEnemy.magnitude;
        float timeToReachEnemy = distanceToEnemy / shotSpeed;
        
        // 敵の予測位置を計算
        Vector2 predictedPosition = enemyPosition + enemyVelocity * timeToReachEnemy;
        
        // 予測位置への方向を計算
        Vector2 fireDirection = (predictedPosition - firePosition).normalized;
        
        // 弾を生成
        GameObject shotObject = Instantiate(shotPrefab, firePosition, Quaternion.identity);
        
        // 弾の方向を設定（回転も設定）
        float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
        shotObject.transform.rotation = Quaternion.Euler(0, 0, angle);
        
        // ダメージ値を取得
        float shotDamage = damage;
        if (upgradeComponent != null)
        {
            shotDamage = upgradeComponent.GetCurrentDamage();
        }
        
        // デバッグ：ダメージ値を表示
        Debug.Log($"[Tower] 弾を発射 - 設定ダメージ: {shotDamage}");
        
        // シンプル弾スクリプトを取得または追加
        PredictiveShot shot = shotObject.GetComponent<PredictiveShot>();
        if (shot == null)
        {
            Debug.Log("[Tower] PredictiveShotコンポーネントが見つからないため追加");
            shot = shotObject.AddComponent<PredictiveShot>();
        }
        else
        {
            Debug.Log("[Tower] 既存のPredictiveShotコンポーネントを使用");
        }
        
        // ダメージ値を含めて初期化
        shot.Initialize(fireDirection, shotSpeed, shotDamage);
        
        // デバッグライン
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
