using UnityEngine;

public class TowerShoot : MonoBehaviour
{
    // 弾のプレハブ
    public GameObject bulletPrefab;
    
    // タワーのパラメータ
    public float attackDamage = 10f;    // 攻撃力
    public float attackSpeed = 1f;      // 攻撃速度（1秒間に何回攻撃するか）
    
    // 射撃間隔（秒）- attackSpeedから自動計算
    public float fireRate = 1f;
    
    // 弾の速度
    public float bulletSpeed = 5f;
    
    // 敵のタグ名
    public string enemyTag = "Enemy";
    
    // 射撃位置
    public Transform firePoint;
    
    // 次に射撃可能になる時間
    private float nextFireTime = 0f;
    
    // 検出範囲 - これを射程距離として使用
    public float detectionRange = 5f;
    
    // デバッグ用に線を描画するかどうか
    public bool drawDebugLines = true;
    
    void Start()
    {
        // attackSpeedからfireRateを計算
        fireRate = 1f / attackSpeed;
        
        // firePointが設定されていない場合は自分自身を使用
        if (firePoint == null)
        {
            Debug.Log("FirePointが設定されていません。タワー自身から発射します。");
            firePoint = transform;
        }
    }
    
    void Update()
    {
        // 最も近い敵を探す
        GameObject nearestEnemy = FindNearestEnemy();
        
        // 敵が範囲内にいて、射撃間隔を過ぎていれば発射
        if (nearestEnemy != null && Time.time >= nextFireTime)
        {
            // 次の射撃時間を設定
            nextFireTime = Time.time + fireRate;
            
            // 敵の方向を正確に計算（デバッグ用）
            Vector2 targetPosition = nearestEnemy.transform.position;
            Vector2 firePosition = firePoint.position;
            Vector2 direction = (targetPosition - firePosition).normalized;
            
            // デバッグ用に敵への線を描画
            if (drawDebugLines)
            {
                Debug.DrawLine(firePosition, targetPosition, Color.red, 1.0f);
                Debug.Log("敵の方向: " + direction);
            }
            
            // 新しい交点射撃法で弾を発射
            FireInterceptBullet(nearestEnemy);
        }
    }
    
    // 最も近い敵を探す関数
    GameObject FindNearestEnemy()
    {
        // 全ての"Enemy"タグを持つオブジェクトを取得
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        
        if (enemies.Length == 0)
        {
            return null;
        }
        
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity; // 無限大から始める
        
        foreach (GameObject enemy in enemies)
        {
            // 敵との距離を計算
            float distance = Vector2.Distance(transform.position, enemy.transform.position);
            
            // まず最も近い敵を見つける
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearestEnemy = enemy;
            }
        }
        
        // 最も近い敵が見つかったら、それが攻撃範囲内かどうか確認
        if (nearestEnemy != null && nearestDistance <= detectionRange)
        {
            return nearestEnemy;
        }
        else
        {
            return null; // 攻撃範囲内に敵がいない
        }
    }
    
    // 弾を発射する関数（ターゲット指定版）- 元の実装
    void FireBullet(Vector2 direction, Vector2 targetPosition)
    {
        if (bulletPrefab != null && firePoint != null)
        {
            // 弾のプレハブを生成
            GameObject bullet = Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
            
            // TargetSeekコンポーネントを追加（これにより弾が確実に敵を追跡）
            BulletTargetSeek targetSeek = bullet.GetComponent<BulletTargetSeek>();
            if (targetSeek == null)
            {
                targetSeek = bullet.AddComponent<BulletTargetSeek>();
            }
            
            // 目標位置を設定
            targetSeek.targetPosition = targetPosition;
            targetSeek.speed = bulletSpeed;
            targetSeek.damage = attackDamage;
            
            // デバッグ用にトレイルレンダラーを追加
            TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
            if (trail == null)
            {
                trail = bullet.AddComponent<TrailRenderer>();
                trail.startWidth = 0.1f;
                trail.endWidth = 0.05f;
                trail.time = 0.5f;
                trail.startColor = Color.yellow;
                trail.endColor = new Color(1, 0.5f, 0, 0.5f);
            }
            
            Debug.Log("弾を発射しました。目標位置: " + targetPosition + ", 攻撃力: " + attackDamage);
        }
    }
    
    // 交点を計算する関数
    Vector2 CalculateInterceptPosition(Vector2 shooterPosition, float projectileSpeed, 
                                     Vector2 targetPosition, Vector2 targetVelocity)
    {
        // 敵と発射位置の相対位置
        Vector2 relativePosition = targetPosition - shooterPosition;
        
        // 二次方程式の係数を計算
        float a = Vector2.Dot(targetVelocity, targetVelocity) - (projectileSpeed * projectileSpeed);
        float b = 2 * Vector2.Dot(targetVelocity, relativePosition);
        float c = Vector2.Dot(relativePosition, relativePosition);
        
        // 判別式
        float discriminant = b * b - 4 * a * c;
        
        // 解が存在するかチェック
        if (discriminant < 0 || Mathf.Approximately(a, 0f))
        {
            // 解なし（弾が敵に追いつけない）→ 敵の現在位置を返す
            return targetPosition;
        }
        
        // 二次方程式を解く
        float t1 = (-b + Mathf.Sqrt(discriminant)) / (2 * a);
        float t2 = (-b - Mathf.Sqrt(discriminant)) / (2 * a);
        
        // 正の最小時間を選択
        float interceptTime;
        if (t1 > 0 && t2 > 0)
        {
            interceptTime = Mathf.Min(t1, t2);
        }
        else if (t1 > 0)
        {
            interceptTime = t1;
        }
        else if (t2 > 0)
        {
            interceptTime = t2;
        }
        else
        {
            // 両方の解が負の場合、敵の現在位置を返す
            return targetPosition;
        }
        
        // 交点位置を計算
        Vector2 interceptPosition = targetPosition + targetVelocity * interceptTime;
        
        return interceptPosition;
    }
    
    // 交点射撃法による弾の発射
    void FireInterceptBullet(GameObject targetEnemy)
    {
        if (bulletPrefab == null || firePoint == null || targetEnemy == null)
            return;
            
        // 敵の情報を取得
        Vector2 enemyPosition = targetEnemy.transform.position;
        Player enemyMove = targetEnemy.GetComponent<Player>();
        
        if (enemyMove == null)
            return;
        
        // 敵の速度ベクトル（左方向に移動）
        Vector2 enemyVelocity = new Vector2(-enemyMove.speed, 0);
        
        // 発射位置
        Vector2 firePosition = firePoint.position;
        
        // 交点を計算
        Vector2 interceptPosition = CalculateInterceptPosition(
            firePosition,       // 発射位置
            bulletSpeed,        // 弾の速度
            enemyPosition,      // 敵の現在位置
            enemyVelocity       // 敵の速度ベクトル
        );
        
        // デバッグ：交点を視覚化
        Debug.DrawLine(firePosition, interceptPosition, Color.green, 1.0f);
        Debug.Log("計算された交点: " + interceptPosition);
        
        // 弾の発射
        GameObject bullet = Instantiate(bulletPrefab, firePosition, Quaternion.identity);
        
        // 方向ベクトルを計算
        Vector2 directionToIntercept = (interceptPosition - firePosition).normalized;
        
        // DirectMoveBulletコンポーネントを追加
        DirectMoveBullet moveScript = bullet.AddComponent<DirectMoveBullet>();
        moveScript.direction = directionToIntercept;
        moveScript.speed = bulletSpeed;
        moveScript.damage = attackDamage;
        
        // デバッグ用の視覚効果
        TrailRenderer trail = bullet.GetComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = bullet.AddComponent<TrailRenderer>();
            trail.startWidth = 0.1f;
            trail.endWidth = 0.05f;
            trail.time = 0.5f;
            trail.startColor = Color.cyan;  // 色を変えて交点射撃を区別
            trail.endColor = new Color(0, 0.5f, 1f, 0.5f);
        }
        
        Debug.Log("交点射撃弾を発射: 目標点 = " + interceptPosition);
    }
    
    // シンプルな予測射撃
    void FirePredictiveBullet(GameObject targetEnemy)
    {
        if (bulletPrefab == null || firePoint == null || targetEnemy == null)
            return;
            
        // 敵の情報を取得
        Vector2 enemyPosition = targetEnemy.transform.position;
        Player enemyMove = targetEnemy.GetComponent<Player>();
        
        if (enemyMove == null)
            return;
            
        // 敵の速度（左方向に移動）
        float enemySpeed = enemyMove.speed;
        
        // 発射位置
        Vector2 firePosition = firePoint.position;
        
        // 敵までの距離
        float distanceToEnemy = Vector2.Distance(firePosition, enemyPosition);
        
        // 弾が敵に到達する時間
        float timeToReachEnemy = distanceToEnemy / bulletSpeed;
        
        // 敵が移動する距離
        float enemyTravelDistance = enemySpeed * timeToReachEnemy;
        
        // 敵の予想位置（左に移動するため、X座標から引く）
        Vector2 predictedPosition = enemyPosition - new Vector2(enemyTravelDistance, 0);
        
        // 予測位置を視覚化
        if (drawDebugLines)
        {
            Debug.DrawLine(firePosition, predictedPosition, Color.blue, 1.0f);
            Debug.Log("予測位置: " + predictedPosition);
        }
        
        // 弾の発射
        GameObject bullet = Instantiate(bulletPrefab, firePosition, Quaternion.identity);
        
        // 方向ベクトル
        Vector2 directionToPredicted = (predictedPosition - firePosition).normalized;
        
        // DirectMoveBulletコンポーネントを追加
        DirectMoveBullet moveScript = bullet.AddComponent<DirectMoveBullet>();
        moveScript.direction = directionToPredicted;
        moveScript.speed = bulletSpeed;
        moveScript.damage = attackDamage;
        
        // デバッグ用の視覚効果
        TrailRenderer trail = bullet.AddComponent<TrailRenderer>();
        if (trail == null)
        {
            trail = trail = bullet.AddComponent<TrailRenderer>();
            trail.startWidth = 0.1f;
            trail.endWidth = 0.05f;
            trail.time = 0.5f;
            trail.startColor = Color.blue;
            trail.endColor = new Color(0, 0, 1f, 0.5f);
        }
        
        Debug.Log("予測射撃弾を発射: 予測位置 = " + predictedPosition);
    }
    
    // インスペクターでattackSpeedが変更された時の処理
    void OnValidate()
    {
        // attackSpeedが0以下にならないように
        if (attackSpeed <= 0)
        {
            attackSpeed = 0.1f;
        }
        
        // attackSpeedからfireRateを再計算
        fireRate = 1f / attackSpeed;
    }
}
